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
            try
            {
                await _slack.Chat.PostMessage(
                    new Message
                    {
                        Text = request.Message,
                        Channel = request.SlackChannel,
                    },
                    cancellationToken);
            }
            catch (SlackException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("[Controller]/UserLookup")]
        public async Task<ActionResult<User?>> LookupUser(
            [FromQuery] string email,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _slack.Users.LookupByEmail(email, cancellationToken);
                return Ok(user);
            }
            catch (SlackException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.Message);
                return NoContent();
            }
        }

        [HttpGet]
        [Route("[Controller]/UserGroupLookup")]
        public async Task<ActionResult<User?>> LookupUserGroup(
            [FromQuery] string userGroup,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userGroups = await _slack.UserGroupUsers.List(
                    userGroup,
                    includeDisabled: false,
                    cancellationToken);

                var firstUserId = userGroups[0];

                var user = await _slack.Users.Info(firstUserId, includeLocale: true, cancellationToken);

                return Ok(user);
            }
            catch (SlackException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.Message);
                return NoContent();
            }
        }
    }
}
