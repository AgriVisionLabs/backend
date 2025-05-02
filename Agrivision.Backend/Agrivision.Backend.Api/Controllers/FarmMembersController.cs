using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Members.Commands;
using Agrivision.Backend.Application.Features.Members.Contracts;
using Agrivision.Backend.Application.Features.Members.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("/farms/{farmId}/members")]
    [ApiController]
    [Authorize]
    public class FarmMembersController(IMediator mediator) : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> GetMembersAsync([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var query = new GetFarmMembersQuery(userId, farmId);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberAsync([FromRoute] Guid farmId, [FromRoute] string memberId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetFarmMemberQuery(userId, farmId, memberId);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateMemberRoleAsync([FromRoute] Guid farmId, [FromRoute] string userId, [FromBody] UpdateMemberRoleRequest request,CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var command = new UpdateMemberRoleCommand(requesterId, farmId, userId, request.RoleName);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RevokeAccessAsync([FromRoute] Guid farmId,
            [FromRoute] string userId, CancellationToken cancellationToken = default)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RevokeAccessCommand(requesterId, farmId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
