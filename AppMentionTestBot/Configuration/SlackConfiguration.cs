namespace AppMentionTestBot.Configuration;

public record SlackConfiguration
{
    // Bot User OAuth Token
    public required string ApiToken { get; set; }
    public required string AppLevelToken { get; set; }
    public required string SigningSecret { get; set; }
    public int NumberOfConnections { get; set; } = 1;
}
