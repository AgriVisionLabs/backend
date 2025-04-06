using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Features.Field.Queries;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FarmsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("created")]
        public async Task<IActionResult> GetAllCreatedByUserAsync(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var result = await mediator.Send(new GetAllFarmsCreatedByUserIdQuery(userId), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetFarmByIdQuery(id), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("")]
        public async Task<IActionResult> AddAsync([FromBody] CreateFarmRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CreateFarmCommand(request.Name, request.Area, request.Location, request.SoilType, userId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? CreatedAtAction(nameof(GetById), new {id = result.Value.Id}, result.Value) : result.ToProblem(result.Error.ToStatusCode()); //change to created at action though CreatedAtAction(nameof(Get), new { id = response.Id }, response.Adapt<PollResponse>());
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateFarmRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var command = new UpdateFarmCommand(id, request.Name, request.Area, request.Location, request.SoilType, userId );
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
        
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
            
            var command = new DeleteFarmCommand(id, userId);
            var result = await mediator.Send(command, cancellationToken);
            
            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
        
        // get fields
        [HttpGet("{id}/fields")]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var command = new GetAllFieldsByFarmIdQuery(id);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
