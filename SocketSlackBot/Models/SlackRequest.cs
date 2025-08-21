namespace SocketSlackBot.Models;

public class SlackMessageRequest
{
    public required string SlackChannel { get; set; }
    public required string Message { get; set; }
}
