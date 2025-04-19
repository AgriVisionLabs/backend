namespace Agrivision.Backend.Application.Features.Farm.Commands;

public record RevokeRoleCommand
(
    string RequesterId,
    string FarmId
);