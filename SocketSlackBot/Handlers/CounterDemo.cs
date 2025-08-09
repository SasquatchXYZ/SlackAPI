using System.Text.RegularExpressions;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using Button = SlackNet.Blocks.Button;

namespace SocketSlackBot.Handlers;

public partial class CounterDemo : IEventHandler<MessageEvent>, IBlockActionHandler<ButtonAction>
{
    private const string ActionPrefix = "add";
    public const string Add1 = ActionPrefix + "1";
    public const string Add5 = ActionPrefix + "5";
    public const string Add10 = ActionPrefix + "10";
    public const string Trigger = "counter demo";
    private static readonly Regex _counterPattern = CounterRegex();

    private readonly ISlackApiClient _slackApiClient;

    public CounterDemo(ISlackApiClient slackApiClient)
    {
        _slackApiClient = slackApiClient;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains(Trigger, StringComparison.OrdinalIgnoreCase) != true)
            return;

        Console.WriteLine(
            $"{(await _slackApiClient.Users.Info(slackEvent.User)).Name} asked for a counter demo in the {(await _slackApiClient.Conversations.Info(slackEvent.Channel)).Name} channel.");

        await _slackApiClient.Chat.PostMessage(
            new Message
            {
                Channel = slackEvent.Channel,
                Blocks = Blocks,
            });
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        Console.WriteLine(
            $"{request.User.Name} clicked on the Add {action.Value} button in the {request.Channel.Name} channel.");

        var counter = SectionBeforeAddButtons(action, request);
        if (counter is not null)
        {
            var counterText = _counterPattern.Match(counter.Text.Text ?? string.Empty);
            if (counterText.Success)
            {
                var count = int.Parse(counterText.Groups[1].Value);
                var increment = int.Parse(((ButtonAction) request.Action).Value);
                counter.Text = $"Counter: {count + increment}";
                await _slackApiClient.Chat.Update(
                    new MessageUpdate
                    {
                        Ts = request.Message.Ts,
                        Text = request.Message.Text,
                        Blocks = request.Message.Blocks,
                        ChannelId = request.Channel.Id,
                    });
            }
        }
    }

    private static SectionBlock? SectionBeforeAddButtons(
        ButtonAction buttonAction,
        BlockActionRequest blockActionRequest) =>
        blockActionRequest.Message.Blocks
            .TakeWhile(block => block.BlockId != buttonAction.BlockId)
            .LastOrDefault() as SectionBlock;

    private static List<Block> Blocks =>
    [
        new SectionBlock
        {
            Text = "Counter: 0",
        },

        new ActionsBlock
        {
            Elements =
            {
                new Button
                {
                    ActionId = Add1,
                    Value = "1",
                    Text = new PlainText("Add 1"),
                },
                new Button
                {
                    ActionId = Add5,
                    Value = "5",
                    Text = new PlainText("Add 5"),
                },
                new Button
                {
                    ActionId = Add10,
                    Value = "10",
                    Text = new PlainText("Add 10"),
                },
            },
        },
    ];

    [GeneratedRegex("Counter: (\\d+)")]
    private static partial Regex CounterRegex();

}
