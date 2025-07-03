using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Messages.Commands;
using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Application.Features.Messages.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("conversations/{conversationId}")]
    [ApiController]
    [Authorize]
    public class MessagesController(IMediator mediator) : ControllerBase
    {
        [HttpPost("[controller]")]
        public async Task<IActionResult> SendMessage([FromRoute] Guid conversationId, [FromBody] SendMessageRequest request)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new SendMessageCommand(conversationId, requesterId, request.Content);
            var result = await mediator.Send(command);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetMessagesAsync([FromRoute] Guid conversationId,
            CancellationToken cancellationToken)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetMessagesQuery(requesterId, conversationId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
