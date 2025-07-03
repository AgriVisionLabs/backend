using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Conversation.Commands;
using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Application.Features.Conversation.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConversationRequest request, CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CreateConversationCommand(requesterId, request.Name, request.MembersList);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { conversationId = result.Value.Id }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid conversationId,
            CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetConversationByIdQuery(requesterId, conversationId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetAccessibleConversationsQuery(requesterId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("{conversationId}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid conversationId,
            CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RemoveConversationCommand(requesterId, conversationId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("{conversationId}/members/{userId}/admin")]
        public async Task<IActionResult> ToggleAdminAsync([FromRoute] Guid conversationId, [FromRoute] string userId,
            CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new ToggleAdminStatusCommand(requesterId, conversationId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpDelete("{conversationId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember([FromRoute] Guid conversationId, [FromRoute] string userId, CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            var command = new RemoveUserFromConversationCommand(requesterId, conversationId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("{conversationId}/clear")]
        public async Task<IActionResult> ClearConversationAsync([FromRoute] Guid conversationId, CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            var command = new ClearConversationCommand(conversationId, requesterId);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("{conversationId}/members")]
        public async Task<IActionResult> AddMember([FromRoute] Guid conversationId, [FromBody] AddConversationMemberRequest request, CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Unauthorized();

            var command = new AddConversationMembersCommand(requesterId, conversationId, request.MembersList);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
