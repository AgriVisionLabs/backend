using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Invitations.Commands;
using Agrivision.Backend.Application.Features.Invitations.Contracts;
using Agrivision.Backend.Application.Features.Invitations.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("/farms/{farmId}/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("pending")]
        public async Task<IActionResult> GetInvitationsAsync([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetInvitationsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpPost("")]
        public async Task<IActionResult> InviteAsync([FromRoute] Guid farmId, [FromBody] InviteMemberRequest request, CancellationToken cancellationToken = default)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var senderEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(senderEmail))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var senderName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
            if (string.IsNullOrEmpty(senderName))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new InviteMemberCommand(senderId, senderEmail, senderName, farmId, request.Recipient, request.RoleId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("{invitationId}")]
        public async Task<IActionResult> CancelInvitationAsync([FromRoute] Guid farmId, [FromRoute] Guid invitationId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CancelInvitationCommand(farmId, invitationId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
