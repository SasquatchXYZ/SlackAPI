using AppMentionTestBot.Configuration;
using Microsoft.OpenApi.Models;
using SlackNet.AspNetCore;
using SlackNet.SocketMode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AppMentionTestBot",
        Version = "v1",
    });
});

var slackConfig = builder.Configuration.GetSection("Slack").Get<SlackConfiguration>();
if (slackConfig is null)
    throw new Exception("Slack configuration is missing.");

builder.Services.AddSlackNet(aspNetSlackServiceConfiguration => aspNetSlackServiceConfiguration
    .UseApiToken(slackConfig.ApiToken)
    .UseAppLevelToken(slackConfig.AppLevelToken)
    .UseSigningSecret(slackConfig.SigningSecret)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "AppMentionTestBot v1"); });
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
var socketModeConnectionOptions = new SocketModeConnectionOptions
{
    NumberOfConnections = slackConfig.NumberOfConnections,
};

app.UseSlackNet(slackEndpointConfiguration =>
    slackEndpointConfiguration.UseSocketMode(true, socketModeConnectionOptions));

app.Run();
