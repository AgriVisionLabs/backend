using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.Account.Contracts;

public class PfpRequest
{
    public IFormFile Image { get; set; }
}