using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationEvents.Queries;
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
    public class IrrigationEventsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("[controller]")]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid farmId,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var query = new GetAllIrrigationEventsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
