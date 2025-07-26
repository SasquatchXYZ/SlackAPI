namespace TestSlackBot.Models;

public class SlackConfiguration
{
    public required string AccessToken { get; set; }
    public required string SigningSecret { get; set; }
}
