using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackEvents.Events;

namespace SlackEvents;

public class SlackEventValueConverter : JsonConverter
{
    public SlackEventValueConverter() : this(typeof(SlackEventValueConverter).Assembly)
    {
    }

    private readonly ConcurrentDictionary<string, Type> _typeMap = new();

    private SlackEventValueConverter(params Assembly[] assemblies)
    {
        var knownEventTypes = assemblies
            .SelectMany(assembly =>
                assembly.ExportedTypes.Where(type =>
                    typeof(ISlackEvent).IsAssignableFrom(type)))
            .ToList();

        foreach (var eventType in knownEventTypes)
        {
            var attributes = eventType.GetCustomAttributes<SlackEventTypeAttribute>();
            foreach (var attribute in attributes)
            {
                Debug.Assert(attribute is not null, nameof(attribute) + " != null");
                _typeMap.TryAdd(attribute.Type, eventType);
            }
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        var jsonToken = serializer.Deserialize<JToken>(reader);
        Debug.Assert(jsonToken is not null, nameof(jsonToken) + " != null");
        if (jsonToken.Type == JTokenType.Null)
            return null;

        if (jsonToken.Type != JTokenType.Object)
            throw new JsonSerializationException($"Expected object, but got a {jsonToken.Type} instead.");

        var jsonObject = (JObject) jsonToken;
        var jsonTypeValue = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
        if (string.IsNullOrWhiteSpace(jsonTypeValue))
            throw new JsonSerializationException("Missing type property.");

        if (_typeMap.TryGetValue(jsonTypeValue, out var netType))
        {
            if (existingValue is null)
                return jsonObject.ToObject(netType, serializer);

            using var stringReader = new StringReader(jsonObject.ToString(Formatting.None));
            using var jsonReader = new JsonTextReader(stringReader);
            serializer.Populate(jsonReader, existingValue);
            return existingValue;
        }

        return new UnknownSlackEvent(jsonTypeValue, jsonObject);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ISlackEvent);
    }
}
