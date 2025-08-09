using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SocketSlackBot.Handlers;

public class AppHome : IEventHandler<AppHomeOpened>
{
    private readonly ISlackApiClient _slackApiClient;

    public AppHome(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(AppHomeOpened slackEvent)
    {
        if (slackEvent.Tab == AppHomeTab.Home)
        {
            Console.WriteLine(
                $"{(await _slackApiClient.Users.Info(slackEvent.User)).Name} opened the app's Home view.");

            var viewDefinition = new HomeViewDefinition
            {
                Blocks =
                {
                    new SectionBlock
                    {
                        Text = new Markdown($"""
                                             Welcome to the SlackNet example.  Here's what you can do:
                                             - Say "ping" to get back a "pong"
                                             - Say "{CounterDemo.Trigger}" to get the counter demo
                                             - Use the `{EchoDemo.SlashCommand}` slash command to see an echo
                                             """),
                    },
                },
            };

            await _slackApiClient.Views.Publish(
                userId: slackEvent.User,
                viewDefinition: viewDefinition,
                hash: slackEvent.View?.Hash);
        }
    }
}
