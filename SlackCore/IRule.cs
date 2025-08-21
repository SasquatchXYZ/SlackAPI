using SlackEvents.Events;

namespace SlackCore;

public interface IRule
{
    Task<bool> CanHandleEventAsync(ISlackEvent slackEvent, CancellationToken cancellationToken);
    Task HandleEventAsync(ISlackEvent slackEvent, CancellationToken cancellationToken);
}
