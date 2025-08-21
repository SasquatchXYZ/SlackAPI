using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlackEvents.Events;

namespace SlackCore;

public class SlackEventProcessor : BackgroundService
{
    private readonly ISlackEventQueue _slackEventQueue;
    private readonly List<IRule> _rules;
    private readonly ILogger<SlackEventProcessor> _logger;

    private ConcurrentBag<Task> _eventTasks = new();
    private const int MaxConcurrentTasks = 20;

    public SlackEventProcessor(
        ISlackEventQueue slackEventQueue,
        IEnumerable<IRule> rules,
        ILogger<SlackEventProcessor> logger)
    {
        _slackEventQueue = slackEventQueue;
        _rules = rules.ToList();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var slackEvent = _slackEventQueue.Dequeue();
            if (slackEvent is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }

            if (_eventTasks.Count >= MaxConcurrentTasks) await Task.WhenAll(_eventTasks);
            _eventTasks = new ConcurrentBag<Task>(_eventTasks.Where(task => !task.IsCompleted))
            {
                ProcessEvent(slackEvent, stoppingToken),
            };
        }
    }

    private async Task ProcessEvent(ISlackEvent slackEvent, CancellationToken cancellationToken)
    {
        foreach (var rule in _rules)
        {
            try
            {
                if (!await rule.CanHandleEventAsync(slackEvent, cancellationToken)) continue;
                await rule.HandleEventAsync(slackEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing event.");
            }

            break;
        }
    }
}
