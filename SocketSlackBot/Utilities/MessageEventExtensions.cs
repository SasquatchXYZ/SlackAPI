using System.Text.RegularExpressions;
using SlackNet.Events;

namespace SocketSlackBot.Utilities;

public static partial class MessageEventExtensions
{
    private static readonly Regex _userRegex = UserRegex();

    public static bool IsUserTagged(this MessageEvent messageEvent, string userId)
    {
        if (messageEvent.Hidden || messageEvent.Text is null) return false;
        return _userRegex.Matches(messageEvent.Text)
            .Where(i => i.Success && i.Groups["userId"].Success)
            .Any(i => string.Equals(i.Groups["userId"].Value, userId, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsDirectMessage(this MessageEvent messageEvent)
    {
        return Regex.Match(messageEvent.Channel, @"^D\S+$").Success;
    }

    [GeneratedRegex(@"<@(?<userId>[a-zA-Z0-9]+)(?:\|\S*)?>", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex UserRegex();

    public static bool IsDirectMessage(this MessageEventBase messageEvent)
    {
        return Regex.Match(messageEvent.Channel, @"^D\S+$").Success;
    }
}
