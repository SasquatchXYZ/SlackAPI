namespace SlackClient.Models;

public record PostMessageRequest
{
    public required string Channel { get; set; }

    public required string Text { get; set; }
    // public ParseMode Parse { get; set; }
    // public bool LinkNames { get; set; }
    // public IList<Attachment> Attachments { get; set; } = [];
    // public IList<Block> Blocks { get; set; } = [];
    // public bool UnfurlLinks { get; set; }
    // public bool UnfurlMedia { get; set; } = true;
    // public string Username { get; set; }
    // public bool? AsUser { get; set; }
    // public string IconUrl { get; set; }
    // public string IconEmoji { get; set; }
    public string? ThreadTs { get; set; }
    // public bool ReplyBroadcast { get; set; }
    // public MessageMetadata MetadataJson { get; set; }
    // public object MetadataObject { get; set; }
}
