using Microsoft.OpenApi.Models;
using SlackClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestSlackBot",
        Version = "v1",
    });
});

builder.Services.AddSlackClient(builder.Configuration);

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
