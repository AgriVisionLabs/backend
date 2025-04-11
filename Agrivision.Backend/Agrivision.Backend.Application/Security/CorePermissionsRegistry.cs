using Agrivision.Backend.Domain.Abstractions.Consts;

namespace Agrivision.Backend.Application.Security;

public static class CorePermissionsRegistry
{
    public static IReadOnlyList<string?> GetAllPermissions() =>
        typeof(CorePermissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}