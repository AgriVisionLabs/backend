using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Commands;
using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Application.Features.AutomationRules.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("farms/{farmId}")]
    [ApiController]
    [Authorize]
    public class AutomationRulesController(IMediator mediator) : ControllerBase
    {
        [HttpPost("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromBody] AddAutomationRuleRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new AddAutomationRuleCommand(farmId, fieldId, userId, request.Name, request.Type,
                request.MinThresholdValue, request.MaxThresholdValue, request.TargetSensorType, request.StartTime,
                request.EndTime, request.ActiveDays);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { farmId, fieldId, automationRuleId = result.Value.Id }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("fields/{fieldId}/[controller]/{automationRuleId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid automationRuleId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetAutomationRuleByIdQuery(farmId, fieldId, automationRuleId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetByFarmId([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetAutomationRulesByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("fields/{fieldId}/[controller]/{automationRuleId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid automationRuleId, [FromBody] UpdateAutomationRuleCommand request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateAutomationRuleCommand(farmId, fieldId, automationRuleId, userId, request.Name, request.IsEnabled, request.Type, request.MinThresholdValue, request.MaxThresholdValue, request.TargetSensorType, request.StartTime, request.EndTime, request.ActiveDays);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("fields/{fieldId}/[controller]/{automationRuleId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromRoute] Guid automationRuleId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RemoveAutomationRuleCommand(farmId, fieldId, automationRuleId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
