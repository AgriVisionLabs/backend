
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Commands;
public record FarmMembers_Contract
(
    string Email,
    FarmRoles Role
    );
