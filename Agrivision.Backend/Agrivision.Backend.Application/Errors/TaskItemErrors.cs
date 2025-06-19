using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class TaskItemErrors
{
    public static readonly Error CannotAssignToElevatedRoles = new(
        "TaskItemErrors.CannotAssignToElevatedRoles",
        "Only owners can assign tasks to elevated roles like Manager.");
    
    public static readonly Error CannotAssignTasksToSelf = new(
        "TaskItemErrors.CannotAssignTaskToSelf",
        "Cannot assign tasks to self.");
    
    public static readonly Error CannotAssignTasksToExpert = new(
        "TaskItemErrors.CannotAssignTaskToExpert",
        "Cannot assigns tasks to expert.");
    
    public static readonly Error DuplicateTitle = new(
        "TaskItemErrors.DuplicateTitle",
        "Duplicate task title exists in the field for an incompleted task.");
    
    public static readonly Error TaskItemNotFound = new(
        "TaskItemErrors.TaskItemNotFound",
        "Task not found.");
}
