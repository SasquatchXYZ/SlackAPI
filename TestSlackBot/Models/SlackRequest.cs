namespace TestSlackBot.Models;

public record SlackRequest(
    string Channel,
    string Message);
