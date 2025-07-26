using Microsoft.OpenApi.Models;
using SlackNet.AspNetCore;
using SlackNet.Events;
using TestSlackBot.Handlers;
using TestSlackBot.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TestSlackBot", Version = "v1" });
});

var slackConfiguration = builder.Configuration.GetSection("Slack").Get<SlackConfiguration>();
if (slackConfiguration is null)
    throw new Exception("Slack configuration is missing.");

builder.Services.AddSingleton(new SlackEndpointConfiguration());

builder.Services.AddSlackNet(configuration => configuration
    .UseApiToken(slackConfiguration.AccessToken)
    .UseSigningSecret(slackConfiguration.SigningSecret)
    .RegisterEventHandler<MessageEvent, PingHandler>()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestSlackBot v1"); });
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
