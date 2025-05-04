
using System.Security.Claims;
using Agrivision.Backend.Application.Features.DiseaseDetections.Commands;
using Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
using Agrivision.Backend.Application.Services.FileManagement;
using Agrivision.Backend.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers;


[Route("/farms/{farmId}/fields/{fieldId}/[controller]")]
//[Route("[controller]")]
[ApiController]
public class DiseaseDetectionsController(IMediator mediator) : ControllerBase
{
    [HttpPost()]
    public async Task<IActionResult> NewDetection([FromForm] DetectionRequest request,[FromRoute] Guid farmId, [FromRoute] Guid fieldId)
    {
         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var command = new CreateDetectionCommand(request.Image, farmId, fieldId,userId); ;
        var result = await mediator.Send(command);
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
      
    }
    
}
