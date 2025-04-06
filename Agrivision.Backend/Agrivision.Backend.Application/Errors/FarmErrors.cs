using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmErrors
{
    public static readonly Error DuplicateFarmName =
        new("Farm.DuplicateName", "User already have a farm with this name.");
    public static readonly Error FarmNotFound = new("Farm.NotFound", "The requested farm was not found.");
    public static readonly Error InvalidFarmId = new("Farm.InvalidFarmId", "The provided farm ID is invalid.");
    public static readonly Error UnauthorizedAction = new("Farm.UnauthorizedAction", "User is not authorized to perform this action.");
}