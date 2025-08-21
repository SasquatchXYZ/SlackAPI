using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SlackCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlackCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISlackEventQueue, SlackEventQueue>();
        services.AddHostedService<SlackEventProcessor>();
        services.Configure<CommandConfiguration>(configuration.GetSection("CommandConfiguration"));
        return services;
    }

    public static IServiceCollection AddRule<T>(this IServiceCollection services) where T : class, IRule =>
        services.AddSingleton<IRule, T>();
}
