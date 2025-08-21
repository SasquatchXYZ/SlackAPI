using static SlackCLI.Utilities.PrintHelper;
using System.CommandLine;
using SlackNet;

namespace SlackCLI.Commands;

public class AuthCommand : Command
{
    public AuthCommand(string name, string? description = null)
        : base(name, description)
    {
        SetAction(async parseResult =>
        {
            var authToken = parseResult.GetValue<string>("--token");
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
