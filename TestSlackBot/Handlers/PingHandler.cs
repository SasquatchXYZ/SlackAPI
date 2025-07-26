using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace TestSlackBot.Handlers;

public class PingHandler : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slackApiClient;

    public PingHandler(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text.Contains("ping"))
        {
            var message = new Message { Text = "pong", Channel = slackEvent.Channel };
            await _slackApiClient.Chat.PostMessage(message).ConfigureAwait(false);
        }
    }
}
