using System.Collections.Concurrent;
using SlackEvents.Events;

namespace SlackCore;

public interface ISlackEventQueue
{
    void Enqueue(ISlackEvent slackEvent);
    ISlackEvent? Dequeue();
}

public class SlackEventQueue : ISlackEventQueue
{
    private readonly ConcurrentQueue<ISlackEvent> _queue = new();

    public void Enqueue(ISlackEvent slackEvent)
    {
        _queue.Enqueue(slackEvent);
    }

    public ISlackEvent? Dequeue()
    {
        var found = _queue.TryDequeue(out var slackEvent);
        return found
            ? slackEvent
            : null;
    }
}
