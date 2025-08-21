namespace SlackEvents;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SlackEventTypeAttribute : Attribute
{
    public SlackEventTypeAttribute(string type)
    {
        Type = type;
    }

    public string Type { get; }
}
