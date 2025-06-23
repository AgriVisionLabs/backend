using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
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
    public class InventoryItemsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("[controller]")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromQuery] Guid? fieldId, [FromBody] AddInventoryItemRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new AddInventoryItemCommand(farmId, fieldId, userId, request.Name, request.Category,
                request.Quantity, request.ThresholdQuantity, request.UnitCost, request.MeasurementUnit, request.ExpirationDate);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

    }
}
