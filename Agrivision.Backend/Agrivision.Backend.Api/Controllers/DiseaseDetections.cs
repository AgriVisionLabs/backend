using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.DiseaseDetection.Commands;
using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Application.Features.DiseaseDetection.Queries;
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
    public class DiseaseDetections(IMediator mediator) : ControllerBase
    {
        [HttpPost("fields/{fieldId}/[controller]")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid farmId, [FromRoute] Guid fieldId,
            [FromForm] AddDiseaseDetectionRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new AddDiseaseDetectionCommand
                { FarmId = farmId, FieldId = fieldId, Image = request.Image, ReqeusterId = userId };
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetAllByFarmIdAsync([FromRoute] Guid farmId,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var query = new GetAllDetectionByFarmIdQuery(farmId, userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
