using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Field.Commands;
using Agrivision.Backend.Application.Features.Field.Contracts;
using Agrivision.Backend.Application.Features.Field.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("/farms/{farmId}/[controller]")]
    [ApiController]
    [Authorize]
    public class FieldsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid farmId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            var command = new GetAllFieldsByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpGet("{fieldId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            var command = new GetFieldByIdQuery(userId, farmId, fieldId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpPost("")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromBody] CreateFieldRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CreateFieldCommand(request.Name, request.Area, farmId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { farmId, fieldId = result.Value.Id }, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("{fieldId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId, [FromBody] UpdateFieldRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateFieldCommand(farmId, fieldId, request.Name, request.Area, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("{fieldId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new DeleteFieldCommand(farmId, fieldId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
