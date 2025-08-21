namespace SlackEvents;

public interface IEventSerializer
{
    Task<byte[]> SerializeAsync<T>(T obj);
    Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken);
}
