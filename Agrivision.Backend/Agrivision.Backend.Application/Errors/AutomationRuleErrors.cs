using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class AutomationRuleErrors
{
    // duplicate name 
    public static readonly Error DuplicateNameInFarm = new("AutomationRuleErrors.DuplicateNameInFarm",
        "An automation rule with the same name already exists in the specified farm.");
    
    // not found
    public static readonly Error AutomationRuleNotFound = new("AutomationRuleErrors.AutomationRuleNotFound",
        "The specified automation rule does not exist in the system.");
}