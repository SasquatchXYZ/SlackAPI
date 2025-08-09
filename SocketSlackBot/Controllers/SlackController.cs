using Microsoft.AspNetCore.Mvc;
using SlackNet;
using SocketSlackBot.Models;
using Message = SlackNet.WebApi.Message;

namespace SocketSlackBot.Controllers
{
    [ApiController]
    public class SlackController : ControllerBase
    {
        private readonly ISlackApiClient _slack;

        public SlackController(
            ISlackApiClient slack)
        {
            _slack = slack;
        }

        [HttpPost]
        [Route("[Controller]/Submit")]
        public async Task<ActionResult> Submit(
            [FromBody] SlackMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            await _slack.Chat.PostMessage(
                new Message
                {
                    Text = request.Message,
                    Channel = request.SlackChannel
                },
                cancellationToken);

            return Ok();
        }
    }
}
