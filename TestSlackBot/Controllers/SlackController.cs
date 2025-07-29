using Microsoft.AspNetCore.Mvc;
using SlackClient;
using SlackClient.Models;
using TestSlackBot.Models;

namespace TestSlackBot.Controllers;

[ApiController]
[Route("[controller]")]
public class SlackController : ControllerBase
{
    private readonly ISlackClient _slackApiClient;
    // private readonly SlackEndpointConfiguration _slackEndpointConfiguration;

    public SlackController(
            ISlackClient slackApiClient)
        // SlackEndpointConfiguration slackEndpointConfiguration,
    {
        _slackApiClient = slackApiClient;
        // _slackEndpointConfiguration = slackEndpointConfiguration;
    }

    [HttpPost]
    [Route("/Submit")]
    public async Task<ActionResult> Submit(
        [FromBody] SlackRequest request,
        CancellationToken cancellationToken = default)
    {
        var message = new PostMessageRequest
        {
            Text = request.Message,
            Channel = request.Channel,
        };

        var response = await _slackApiClient.PostMessage(
            message,
            cancellationToken);

        if (response is not null)
        {
            Console.WriteLine($"Channel: {response?.Channel}");
            Console.WriteLine($"Ts: {response?.Ts}");

            var threadMessage = new PostMessageRequest
            {
                Text = "Testing threaded reply :thumbsup:",
                Channel = request.Channel,
                ThreadTs = response?.Ts,
            };

            var threadResponse = await _slackApiClient.PostMessage(
                threadMessage,
                cancellationToken);
        }

        return Ok();
    }

    // [HttpPost]
    // [Route("/Event")]
    // public async Task<ActionResult> Event()
    // {
    //     var result = await _slackRequestHandler.HandleEventRequest(
    //         HttpContext.Request);
    //     // _slackEndpointConfiguration);
    //
    //     return Ok(result);
    // }
}
