namespace SlackClient.Configuration;

public class SlackConfiguration
{
    public required string Url { get; init; }
    public required string AccessToken { get; init; }
    public required string SigningSecret { get; init; }
    public int MaxRetries { get; init; } = 3;
    public int TimeoutMs { get; init; } = 10000;
    public int MaxBackoffMs { get; init; } = 10000;
}
