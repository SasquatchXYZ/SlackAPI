using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SocketSlackBot.Handlers
{
    public class PingHandler : IEventHandler<MessageEvent>
    {
        private readonly ILogger<PingHandler> _logger;
        private readonly ISlackApiClient _slack;

        public PingHandler(
            ILogger<PingHandler> logger,
            ISlackApiClient slack)
        {
            _logger = logger;
            _slack = slack;
        }

        public async Task Handle(MessageEvent slackEvent)
        {
            if (slackEvent.Text?.Contains("ping", StringComparison.OrdinalIgnoreCase) != true)
                return;

            _logger.LogInformation("Received ping from {User} in the {Channel} channel",
                (await _slack.Users.Info(slackEvent.User)).Name,
                (await _slack.Conversations.Info(slackEvent.Channel)).Name);

            await _slack.Chat.PostMessage(
                new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel,
                });
        }
    }
}
