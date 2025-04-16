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
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FieldsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{fieldId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid fieldId,
            CancellationToken cancellationToken = default)
        {
            var command = new GetFieldByIdQuery(fieldId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
        
        [HttpPost("")]
        public async Task<IActionResult> AddAsync([FromBody] CreateFieldRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CreateFieldCommand(request.Name, request.Area, request.FarmId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new {id = result.Value.Id}, result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPut("{fieldId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid fieldId, [FromBody] UpdateFieldRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new UpdateFieldCommand(fieldId, request.Name, request.Area, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpDelete("{fieldId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid fieldId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new DeleteFieldCommand(fieldId, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
