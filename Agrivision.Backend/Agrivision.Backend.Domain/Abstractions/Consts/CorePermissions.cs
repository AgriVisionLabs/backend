namespace Agrivision.Backend.Domain.Abstractions.Consts;

public static class CorePermissions
{
    public static string Type { get; } = "core-permissions";
    
    // farm management
    public const string ViewFarm = "core:farms:view";
    public const string UpdateFarm = "core:farms:update";
    public const string DeleteFarm = "core:farms:delete";
    
    // field management
    public const string UpdateField = "core:fields:update";
    public const string DeleteField = "core:fields:delete";
    public const string ViewFields = "core:fields:view";

    // role and user assignment
    public const string ViewFarmUsers = "core:users:view";
    public const string AssignFarmRoles = "core:users:assign-role";
}