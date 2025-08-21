using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlackEvents;

public class EventSerializer : IEventSerializer
{
    private readonly JsonSerializer _serializer;

    public EventSerializer()
    {
        _serializer = new JsonSerializer();
        _serializer.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        };

        _serializer.Converters.Add(new SlackEventValueConverter());
        _serializer.NullValueHandling = NullValueHandling.Ignore;
    }

    public async Task<byte[]> SerializeAsync<T>(T obj)
    {
        await using var memoryStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(memoryStream);

        _serializer.Serialize(streamWriter, obj);
        streamWriter.Close();
        return memoryStream.GetBuffer().ToArray();
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        using var jsonReader = new JsonTextReader(streamReader);

        return Task.FromResult(_serializer.Deserialize<T>(jsonReader));
    }
}
