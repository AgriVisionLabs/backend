using Agrivision.Backend.Domain.Abstractions.Consts;

namespace Agrivision.Backend.Application.Security;

public static class GlobalPermissionsRegistry
{
    public static IReadOnlyList<string?> GetAllPermissions() =>
        typeof(GlobalPermissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}