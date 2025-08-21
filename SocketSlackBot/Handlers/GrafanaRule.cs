using System.Text.RegularExpressions;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using SocketSlackBot.Utilities;

namespace SocketSlackBot.Handlers;

public partial class GrafanaRule : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slackApiClient;

    private readonly string _hostNameRegex = "examplehostname";
    private string RegexString => $@"(?<link><https?://(www\.)?({_hostNameRegex})[^|\s]+(\|\S*)?>)";
    private const string BotUserId = "U099NKKH79U";

    public GrafanaRule(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(MessageEvent slackEvent)
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

    private bool ShouldHandle(MessageEvent slackEvent)
    {
        if (slackEvent.Hidden || slackEvent.Text is null ||
            !slackEvent.IsDirectMessage() && !slackEvent.IsUserTagged(BotUserId))
            return false;

        return Regex.Match(slackEvent.Text, RegexString).Success;
    }

    private static bool ShouldThread(MessageEvent slackEvent) =>
        !(slackEvent.IsDirectMessage() || ShouldThreadRegex().Match(slackEvent.Text).Success);

    [GeneratedRegex(@"\bnothread\b")]
    private static partial Regex ShouldThreadRegex();
}
