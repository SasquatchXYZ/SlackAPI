namespace SlackEvents.Events;

public interface ISlackEvent
{
    string Type { get; }
}
