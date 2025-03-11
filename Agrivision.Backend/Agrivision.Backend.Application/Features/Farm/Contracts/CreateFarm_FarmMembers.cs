
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Commands;
public record CreateFarm_FarmMembers
(
    string Email,
    FarmRoles Role
    );
