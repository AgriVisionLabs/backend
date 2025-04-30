using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Application.Features.IrrigationUnits.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("/farms/{farmId}")]
    [ApiController]
    [Authorize]
    public class IrrigationUnitsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromBody] AddIrrigationUnitRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            var userName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
            if (string.IsNullOrEmpty(userName))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command =
                new AddIrrigationUnitCommand(farmId, fieldId, userId, userName, request.SerialNumber, request.Name);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { farmId, fieldId }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("fields/{fieldId}/[controller]/toggle")]
        public async Task<IActionResult> ToggleAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var userName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
            if (string.IsNullOrEmpty(userName))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new ToggleIrrigationUnitCommand(farmId, fieldId, userId, userName);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId, [FromBody] UpdateIrrigationUnitRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateIrrigationUnitCommand(farmId, fieldId, userId, request.Name, request.Status,
                request.NewFieldId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RemoveIrrigationUnitCommand(farmId, fieldId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> GetById([FromRoute] Guid farmId, [FromRoute] Guid fieldId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetIrrigationUnitByFieldIdQuery(farmId, fieldId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetIrrigationUnitsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
