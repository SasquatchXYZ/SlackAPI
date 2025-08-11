using Microsoft.OpenApi.Models;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.SocketMode;
using SocketSlackBot.Handlers;
using SocketSlackBot.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SocketSlackBot",
        Version = "v1",
    });
});

var slackConfig = builder.Configuration.GetSection("Slack").Get<SlackConfiguration>();
if (slackConfig is null)
    throw new Exception("Slack configuration is missing.");

builder.Services.AddSlackNet(c => c
    .UseApiToken(slackConfig.ApiToken)
    .UseAppLevelToken(slackConfig.AppLevelToken)
    .UseSigningSecret(slackConfig.SigningSecret)

    // App Home Screen Demo
    // Event `app_home_opened` must be enabled for this to work
    // You also must enable the `Home Tab` under the `App Home` Features for the bot
    .RegisterEventHandler<AppHomeOpened, AppHome>()

    // Ping/Pong Demo
    .RegisterEventHandler<MessageEvent, PingHandler>()

    // Counter Demo - Interactive Block message that updates itself
    .RegisterEventHandler<MessageEvent, CounterDemo>()
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add1)
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add5)
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add10)

    // Modal view demo
    .RegisterEventHandler<MessageEvent, ModalViewDemo>()
    .RegisterBlockActionHandler<ButtonAction, ModalViewDemo>(ModalViewDemo.OpenModal)
    .RegisterViewSubmissionHandler<ModalViewDemo>(ModalViewDemo.ModalCallbackId)

    // Simple Slash Command demo that echos the message
    .RegisterSlashCommandHandler<EchoDemo>(EchoDemo.SlashCommand)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "SocketSlackBot v1"); });
app.UseHttpsRedirection();
app.MapControllers();
var socketModeConnectionOptions = new SocketModeConnectionOptions
{
    NumberOfConnections = slackConfig.NumberOfConnections,
};

app.UseSlackNet(slackEndpointConfiguration =>
    slackEndpointConfiguration.UseSocketMode(true, socketModeConnectionOptions));

app.Run();
