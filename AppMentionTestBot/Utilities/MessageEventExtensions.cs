using System.Text.RegularExpressions;
using SlackNet.Events;

namespace AppMentionTestBot.Utilities;

public static partial class MessageEventExtensions
{
    public static bool IsDirectMessage(this MessageEventBase messageEvent)
    {
        return DirectMessageRegex().Match(messageEvent.Channel).Success;
    }

    [GeneratedRegex(@"^D\S+$")]
    private static partial Regex DirectMessageRegex();
}
