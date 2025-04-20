using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Features.FarmRoles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FarmRolesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> GetAllFarmRolesAsync(CancellationToken cancellationToken = default)
        {
            var query = new GetFarmRolesQuery();
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpGet("{roleName}/permissions")]
        public async Task<IActionResult> GetRolePermissionsAsync([FromRoute] string roleName,
            CancellationToken cancellationToken = default)
        {
            var query = new GetFarmRolePermissionsQuery(roleName);
            var result = await mediator.Send(query, cancellationToken);
            
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode()); 
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetRolesPermissionAsync(CancellationToken cancellationToken)
        {
            var query = new GetFarmRolesPermissionsQuery();
            var result = await mediator.Send(query, cancellationToken);

            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
