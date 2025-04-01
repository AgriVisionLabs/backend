using System.Globalization;
using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Infrastructure.Auth.Filters;
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
        //[FarmRole("Manager", "Owner")]
        [FarmRole("Worker")]
        [HttpGet("{farmId}")]
        public async Task<IActionResult> GetById([FromRoute] string farmId, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetFarmByIdQuery(farmId), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("")]
        public async Task<IActionResult> AddAsync([FromBody] FarmRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new CreateFarmCommand(request.Name, request.Area, request.Location, request.SoilType, userId,request.FarmMembers);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode()); //change to created at action though CreatedAtAction(nameof(Get), new { id = response.Id }, response.Adapt<PollResponse>());
        }
    }
}
