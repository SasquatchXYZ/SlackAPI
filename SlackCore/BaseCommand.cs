using Microsoft.Extensions.Options;
using SlackEvents.Events;

namespace SlackCore;

public abstract class BaseCommand : IRule
{
    private readonly CommandConfiguration _commandConfiguration;

    protected BaseCommand(IOptions<CommandConfiguration> commandConfiguration)
    {
        _commandConfiguration = commandConfiguration.Value;
    }

    protected abstract string Name { get; }

    public async Task<bool> CanHandleEventAsync(ISlackEvent slackEvent, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (slackEvent is not MessageEvent messageEvent)
            return false;

        if (messageEvent.Text is null)
            return false;

        var withTrigger = $"{_commandConfiguration.CommandPrefix}{Name}";

        return string.Equals(messageEvent.Text, withTrigger, StringComparison.OrdinalIgnoreCase) ||
               messageEvent.Text.StartsWith($"{withTrigger} ", StringComparison.OrdinalIgnoreCase);
    }

    public async Task HandleEventAsync(ISlackEvent slackEvent, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (slackEvent is not MessageEvent messageEvent)
            throw new InvalidOperationException("Expected a message event in BaseCommand HandleEventAsync..");

        await HandleCommandAsync(
            messageEvent,
            messageEvent.Text?.Substring(_commandConfiguration.CommandPrefix.Length + Name.Length),
            cancellationToken);
    }

    protected abstract Task HandleCommandAsync(
        MessageEvent messageEvent,
        string? commandText,
        CancellationToken cancellationToken);
}
