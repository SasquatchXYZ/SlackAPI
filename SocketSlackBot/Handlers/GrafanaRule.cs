using System.Text.RegularExpressions;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using SocketSlackBot.Utilities;

namespace SocketSlackBot.Handlers;

public class GrafanaRule : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slackApiClient;

    // private readonly string _hostNameRegex = "devgrafana.csnzoo.com|grafana-oss.dev.plat.intranet.wayfair.io";
    private readonly string _hostNameRegex = "examplehostname";
    private string RegexString => $@"(?<link><https?://(www\.)?({_hostNameRegex})[^|\s]+(\|\S*)?>)";
    private readonly string _botUserId = "U099NKKH79U";

    public GrafanaRule(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        if (!ShouldHandle(slackEvent)) return;

        if (slackEvent.Text is null) return;
        var shouldThread = !(slackEvent.IsDirectMessage() || Regex.Match(slackEvent.Text, @"\bnothread\b").Success);
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
            !slackEvent.IsDirectMessage() && !slackEvent.IsUserTagged(_botUserId))
            return false;

        var match = Regex.Match(slackEvent.Text, RegexString);
        var test = match.Success;
        return test;
    }
}
