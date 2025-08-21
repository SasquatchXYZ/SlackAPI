using Newtonsoft.Json;
using Spectre.Console;

namespace SlackCLI.Utilities;

public static class PrintHelper
{
    public static void ShowErrorMessage(string[] errorMessages)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.MarkupLine(
            Emoji.Known.CrossMark + " [bold red]ERROR[/] :cross_mark:"
        );

        foreach (var message in errorMessages)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
        }
    }

    public static void PrintJson(string header, object obj)
    {
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.Write(
            new Panel(json)
                .Header(header)
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Cyan3));
    }
}
