namespace SlackClient.Models;

public class PostMessageResponse
{
    public string Ts { get; set; } = string.Empty;

    public string Channel { get; set; } = string.Empty;
    // public MessageEvent Message { get; set; }
}
