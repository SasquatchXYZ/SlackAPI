using Newtonsoft.Json;

namespace SlackEvents.Events;

// These don't have all available fields
[SlackEventType("message")]
public record MessageEvent : ISlackEvent
{
    private readonly string? _text;

    [JsonProperty("client_msg_id")] public required string ClientMessageId { get; init; }
    public string? Subtype { get; init; }
    public string? User { get; init; }
    public string Type { get; init; } = string.Empty;

    public string? Text
    {
        get => _text;
        init => _text = value?.Replace("\u00a0", " ");
    }

    public required string Channel { get; init; }
    public required MessageEvent Message { get; init; }
    public bool Hidden { get; init; }
    public string? SourceTeam { get; init; }
    public string? UserTeam { get; init; }
    [JsonProperty("thread_ts")] public string? ThreadTimestamp { get; init; }
    [JsonProperty("ts")] public string? Timestamp { get; init; }
    [JsonProperty("event_ts")] public string? EventTimestamp { get; init; }

    [JsonIgnore]
    public string? ParentTimestamp
    {
        get => ThreadTimestamp ?? Timestamp;
    }

    public string? BotId { get; init; }
    public Attachment[] Attachments { get; init; } = [];
}

public record Attachment(
    string Color,
    string Fallback,
    string ImageUrl,
    string Text,
    string Title,
    string TitleLink
);
