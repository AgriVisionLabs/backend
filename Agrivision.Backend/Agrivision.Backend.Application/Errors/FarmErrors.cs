using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmErrors
{
    public static readonly Error DuplicateFarmName =
        new("Farm.DuplicateName", "User already have a farm with this name.");
    public static readonly Error NoFarmsFound = new("Farm.NoFarmsFound", "No farms were found for this user.");
}