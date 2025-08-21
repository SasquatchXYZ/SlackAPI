using System.CommandLine;
using SlackCLI.Commands;

var rootCommand = new RootCommand("Helpful tool for various Slack API interactions.");

rootCommand.Add(new AuthCommand("auth", "Authenticates with Slack and prints the user's information."));

return await rootCommand.Parse(args).InvokeAsync();
