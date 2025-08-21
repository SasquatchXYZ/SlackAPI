using System.CommandLine;
using SlackCLI.Commands;

Option<string> tokenOption = new(
    "--token",
    ["-t"])
{
    Required = true,
    Arity = ArgumentArity.ExactlyOne,
    AllowMultipleArgumentsPerToken = false,
    Recursive = true,
};

var rootCommand = new RootCommand("Helpful tool for various Slack API interactions.");
rootCommand.Add(tokenOption);

rootCommand.Add(new AuthCommand("auth", "Authenticates with Slack and prints the user's information."));

return await rootCommand.Parse(args).InvokeAsync();
