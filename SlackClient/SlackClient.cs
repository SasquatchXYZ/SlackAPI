using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SlackClient.Models;

namespace SlackClient;

public interface ISlackClient
{
    Task<PostMessageResponse?> PostMessage(PostMessageRequest request, CancellationToken cancellationToken);
}

public class SlackClient : ISlackClient
{
    private readonly HttpClient _httpClient;

    public SlackClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<PostMessageResponse?> PostMessage(
        PostMessageRequest request,
        CancellationToken cancellationToken)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
        };

        var serialized = JsonConvert.SerializeObject(request, serializerSettings);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat.postMessage", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<PostMessageResponse>(responseContent);
    }
}
