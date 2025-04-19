using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Commands;

public record UpdateMemberRoleCommand
(
    string RequesterId,
    Guid FarmId,
    string UserId,
    int RoleId
) : IRequest<Result>;