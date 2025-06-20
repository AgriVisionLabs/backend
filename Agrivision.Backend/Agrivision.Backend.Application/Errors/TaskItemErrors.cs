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
    
    public static readonly Error UnauthorizedAction = new(
        "TaskItemErrors.UnauthorizedAction",
        "Unauthorized action.");
        
    public static readonly Error AlreadyClaimed = new(
        "TaskItemErrors.AlreadyClaimed",
        "Task already claimed by another user.");
    
    public static readonly Error TaskCannotBeCompletedUnclaimed = new(
        "TaskItemErrors.TaskCannotBeCompletedUnclaimed",
        "Unassigned and unclaimed tasks cannot be completed.");
    
    public static readonly Error TaskCannotBeCompletedByAnotherUser = new(
        "TaskItemErrors.TaskCannotBeCompletedByAnotherUser",
        "Only the assigned or claimed user can complete this task.");

    public static readonly Error TaskAlreadyCompleted = new(
        "TaskItemErrors.TaskAlreadyCompleted",
        "This task has already been completed.");
}
