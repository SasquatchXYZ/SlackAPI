using Newtonsoft.Json.Linq;

namespace SlackEvents.Events;

public record UnknownSlackEvent(string Type, JObject JsonObject) : ISlackEvent;
