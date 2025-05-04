using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Commands;
using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Application.Features.SensorUnits.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("farms/{farmId}")]
    [ApiController]
    [Authorize]
    public class SensorUnitsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId, [FromBody] AddSensorUnitRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            var userName = User.FindFirstValue(JwtRegisteredClaimNames.Name);
            if (string.IsNullOrEmpty(userName))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command =
                new AddSensorUnitCommand(farmId, fieldId, userId, userName, request.SerialNumber, request.Name);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { farmId, fieldId, sensorUnitId = result.Value.Id }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("fields/{fieldId}/[controller]/{sensorUnitId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid sensorUnitId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetSensorUnitByIdQuery(farmId, fieldId, sensorUnitId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetByFarmId([FromRoute] Guid farmId,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetSensorUnitsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpPut("fields/{fieldId}/[controller]/{sensorUnitId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid sensorUnitId, [FromBody] UpdateSensorUnitRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateSensorUnitCommand(farmId, fieldId, sensorUnitId, userId,request.Name, request.Status, request.NewFieldId);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("fields/{fieldId}/[controller]/{sensorUnitId}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid sensorUnitId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RemoveSensorUnitCommand(farmId, fieldId, sensorUnitId, userId);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
        
    }
}
