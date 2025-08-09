using Microsoft.OpenApi.Models;
using SlackNet.AspNetCore;
using SlackNet.Events;
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
    .RegisterEventHandler<MessageEvent, PingHandler>()
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
app.UseSlackNet(slackEndpointConfiguration => slackEndpointConfiguration.UseSocketMode(true));

app.Run();
