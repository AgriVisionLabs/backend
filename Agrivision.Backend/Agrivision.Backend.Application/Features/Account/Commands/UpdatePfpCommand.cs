using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.Account.Commands;

public class UpdatePfpCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = default!;
    public IFormFile Image { get; set; }
}
