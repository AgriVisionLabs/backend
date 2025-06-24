using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Inventory.Commands;
using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Application.Features.Inventory.Queries;
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
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromBody] AddInventoryItemRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new AddInventoryItemCommand(farmId, request.FieldId, userId, request.Name, request.Category,
                request.Quantity, request.ThresholdQuantity, request.UnitCost, request.MeasurementUnit, request.ExpirationDate);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { farmId = result.Value.FarmId, itemId = result.Value.Id }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetAllInventoryItemsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]/{itemId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid farmId, [FromRoute] Guid itemId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetInventoryItemByIdQuery(farmId, itemId, userId);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("[controller]/{itemId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid farmId, [FromRoute] Guid itemId, [FromBody] UpdateInventoryItemRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateInventoryItemCommand(farmId, request.FieldId, itemId, userId, request.Name,
                request.Category, request.Quantity, request.ThresholdQuantity, request.UnitCost,
                request.MeasurementUnit, request.ExpirationDate);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode()); 
        }

        [HttpPost("[controller]/{itemId}/log")]
        public async Task<IActionResult> LogChangeAsync([FromRoute] Guid farmId, [FromRoute] Guid itemId, [FromBody] InventoryItemLogChangeRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new InventoryItemLogChangeCommand(farmId, itemId, userId, request.Quantity, request.Reason);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("[controller]/{itemId}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid farmId, [FromRoute] Guid itemId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new RemoveInventoryItemCommand(farmId, itemId, userId);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
