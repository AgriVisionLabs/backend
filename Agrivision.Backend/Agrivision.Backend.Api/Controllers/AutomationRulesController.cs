using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Commands;
using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
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
            
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
