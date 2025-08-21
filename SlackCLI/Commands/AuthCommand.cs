using static SlackCLI.Utilities.PrintHelper;
using System.CommandLine;
using SlackNet;

namespace SlackCLI.Commands;

public class AuthCommand : Command
{
    private readonly Option<string> _tokenOption = new(
        "--token",
        ["-t"])
    {
        Required = true,
        Arity = ArgumentArity.ExactlyOne,
        AllowMultipleArgumentsPerToken = false,
    };

    public AuthCommand(string name, string? description = null)
        : base(name, description)
    {
        Add(_tokenOption);

        SetAction(async parseResult =>
        {
            var authToken = parseResult.GetValue(_tokenOption);
            var slackApiClient = new SlackServiceBuilder()
                .UseApiToken(authToken)
                .GetApiClient();

            await OnHandleAuthCommand(slackApiClient);
        });
    }

    private static async Task OnHandleAuthCommand(ISlackApiClient slackApiClient)
    {
        var response = await slackApiClient.Auth.Test();
        PrintJson("Auth Test", response);
    }
}
