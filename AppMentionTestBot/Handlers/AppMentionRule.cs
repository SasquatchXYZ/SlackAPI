using static AppMentionTestBot.Utilities.MessageEventExtensions;
using System.Text.RegularExpressions;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace AppMentionTestBot.Handlers;

public class AppMentionRule : IEventHandler<AppMention>
{
    private readonly ISlackApiClient _slackApiClient;

    private const string HostNameRegex = "examplehostname";

    private static string RegexString => $@"(?<link><https?://(www\.)?({HostNameRegex})[^|\s]+(\|\S*)?>)";

    public AppMentionRule(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(AppMention slackEvent)
    {
        if (!ShouldHandle(slackEvent)) return;

        if (slackEvent.Text is null) return;
        var shouldThread = ShouldThread(slackEvent);
        var matches = Regex.Matches(slackEvent.Text, RegexString);
        await _slackApiClient.Chat.PostMessage(
            new Message
            {
                Text = $"Matches: {string.Join(", ", matches.Select(m => m.Value))}",
                Channel = slackEvent.Channel,
                ThreadTs = shouldThread ? slackEvent.Ts : null,
            });
    }

    private static bool ShouldHandle(AppMention slackEvent)
    {
        if (slackEvent.Text is null || slackEvent.IsDirectMessage())
            return false;

        // if (slackEvent.Hidden || slackEvent.Text is null ||
        //     !slackEvent.IsDirectMessage() && !slackEvent.IsUserTagged(_botUserId))
        //     return false;

        var match = Regex.Match(slackEvent.Text, RegexString);
        var test = match.Success;
        return test;
    }

    private static bool ShouldThread(AppMention slackEvent)
    {
        return !(slackEvent.IsDirectMessage() || Regex.Match(slackEvent.Text, @"\bnothread\b").Success);
    }
}
