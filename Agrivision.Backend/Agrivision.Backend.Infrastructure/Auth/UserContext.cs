using System.Security.Claims;
using Agrivision.Backend.Application.Auth;
using Microsoft.AspNetCore.Http;



namespace Agrivision.Backend.Infrastructure.Auth;
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string UserId =>
      _contextAccessor.HttpContext?
      .User.FindFirstValue(ClaimTypes.NameIdentifier) ??
      throw new ApplicationException("User context is unavailable");
}