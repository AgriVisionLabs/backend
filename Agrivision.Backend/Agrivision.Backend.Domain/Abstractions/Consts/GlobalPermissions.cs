namespace Agrivision.Backend.Domain.Abstractions.Consts;

public static class GlobalPermissions
{
    public static string Type { get; } = "global-permissions";
    
    public const string ReadUsers = "global:users:read";
    public const string CreateUsers = "global:users:create";
    public const string UpdateUsers = "global:users:update";
    public const string DeleteUsers = "global:users:delete";

    // global role management
    public const string ReadGlobalRoles = "global:roles:read";
    public const string CreateGlobalRoles = "global:roles:create";
    public const string UpdateGlobalRoles = "global:roles:update";
    public const string DeleteGlobalRoles = "global:roles:delete";

    // core (farm) role management
    public const string ReadCoreRoles = "core:roles:read";
    public const string CreateCoreRoles = "core:roles:create";
    public const string UpdateCoreRoles = "core:roles:update";
    public const string DeleteCoreRoles = "core:roles:delete";

    public const string ViewAnyFarm = "global:farms:read";
    public const string UpdateAnyFarm = "global:farms:update";
    public const string DeleteAnyFarm = "global:farms:delete";
    
    public const string AuditLogAccess = "admin:auditlogs:read";
    public const string ToggleFeatures = "admin:features:toggle"; // enable or disable experimental features
    public const string ImpersonateUser = "support:impersonate"; // log in as a user for debugging
}