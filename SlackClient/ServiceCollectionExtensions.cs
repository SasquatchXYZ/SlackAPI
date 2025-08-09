using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using SlackClient.Configuration;

namespace SlackClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlackClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SlackConfiguration>()
            .Bind(configuration.GetSection("Slack"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<ISlackClient, SlackClient>(client =>
        {
            var config = configuration.GetSection("Slack").Get<SlackConfiguration>();

            if (config is null)
                throw new Exception("Slack configuration is missing.");

            client.BaseAddress = new Uri(config.Url);
            client.DefaultRequestVersion = new Version(2, 0);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            EnableMultipleHttp2Connections = true,
        }).AddRetryAndTimeoutPolicy();

        return services;
    }

    private static IHttpClientBuilder AddRetryAndTimeoutPolicy(this IHttpClientBuilder httpClientBuilder)
    {
        httpClientBuilder.AddRetryAndTimeoutPolicy((serviceProvider, _) =>
            serviceProvider.GetRequiredService<IOptionsMonitor<SlackConfiguration>>()
                .Get(typeof(SlackConfiguration).FullName));

        return httpClientBuilder;
    }

    private static IHttpClientBuilder AddRetryAndTimeoutPolicy(
        this IHttpClientBuilder httpClientBuilder,
        Func<IServiceProvider, HttpRequestMessage, SlackConfiguration> getParams) =>
        httpClientBuilder.AddPolicyHandler((provider, httpRequestMessage) =>
        {
            var clientConfig = getParams(provider, httpRequestMessage);
            return GetRetryAndTimeoutPolicy(clientConfig);
        });

    private static AsyncPolicyWrap<HttpResponseMessage>
        GetRetryAndTimeoutPolicy(SlackConfiguration config) =>
        Policy.WrapAsync(
            config.MaxBackoffMs is { } backoff
                ? GetRetryWithJitterPolicy(config.MaxRetries, backoff)
                : GetRetryPolicy(config.MaxRetries),
            GetTimeoutPolicy(config.TimeoutMs));


    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(int maxRetries) =>
        TransientErrorHandlingPolicy()
            .RetryAsync(maxRetries);

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryWithJitterPolicy(int maxRetries, int maxBackoffMs) =>
        TransientErrorHandlingPolicy()
            .WaitAndRetryAsync(maxRetries, _ => TimeSpan.FromMilliseconds(Random.Shared.Next(0, maxBackoffMs)));

    private static PolicyBuilder<HttpResponseMessage> TransientErrorHandlingPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(httpResponseMessage => httpResponseMessage.StatusCode is HttpStatusCode.BadGateway or
                HttpStatusCode.GatewayTimeout or
                HttpStatusCode.ServiceUnavailable)
            .Or<TimeoutRejectedException>()
            .Or<ObjectDisposedException>();

    private static AsyncTimeoutPolicy<HttpResponseMessage> GetTimeoutPolicy(int timeoutMs) =>
        Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(timeoutMs), TimeoutStrategy.Optimistic);
}
