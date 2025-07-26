using Microsoft.AspNetCore.Mvc;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.WebApi;
using TestSlackBot.Models;

namespace TestSlackBot.Controllers;

[ApiController]
[Route("[controller]")]
public class SlackController : ControllerBase
{
    private readonly ISlackRequestHandler _slackRequestHandler;
    private readonly ISlackApiClient _slackApiClient;
    // private readonly SlackEndpointConfiguration _slackEndpointConfiguration;

    public SlackController(
            ISlackRequestHandler slackRequestHandler,
            ISlackApiClient slackApiClient)
        // SlackEndpointConfiguration slackEndpointConfiguration,
    {
        _slackRequestHandler = slackRequestHandler;
        _slackApiClient = slackApiClient;
        // _slackEndpointConfiguration = slackEndpointConfiguration;
    }

    [HttpPost]
    [Route("/Submit")]
    public async Task<ActionResult> Submit(
        [FromBody] SlackRequest request,
        CancellationToken cancellationToken = default)
    {
        var message = new Message { Text = request.Message, Channel = request.Channel };
        await _slackApiClient.Chat.PostMessage(
            message,
            cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("/Event")]
    public async Task<ActionResult> Event()
    {
        var result = await _slackRequestHandler.HandleEventRequest(
            HttpContext.Request);
        // _slackEndpointConfiguration);

        return Ok(result);
    }
}
