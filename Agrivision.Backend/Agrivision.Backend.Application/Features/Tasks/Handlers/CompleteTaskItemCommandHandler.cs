using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class CompleteTaskItemCommandHandler(ITaskItemRepository taskItemRepository, IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<CompleteTaskItemCommand, Result>
{
    public async Task<Result> Handle(CompleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // check if user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, farm.Id, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // expert can't see tasks
        if (farmUserRole.FarmRole.Name is "Expert")
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // get the task 
        var task = await taskItemRepository.FindByIdAsync(request.TaskItemId, cancellationToken);
        if (task is null)
            return Result.Failure(TaskItemErrors.TaskItemNotFound);
        
        // check if the task belong to the farm
        if (task.Field.FarmId != request.FarmId)
            return Result.Failure(TaskItemErrors.TaskItemNotFound);
        
        // unassigned and unclaimed tasks cannot be completed
        if (task.AssignedToId is null && task.ClaimedById is null)
            return Result.Failure(TaskItemErrors.TaskCannotBeCompletedUnclaimed);
        
        // tasks cannot be marked completed except by the user it is assigned to or claimed by 
        var isOwner = task.AssignedToId == request.RequesterId || task.ClaimedById == request.RequesterId;
        if (!isOwner)
            return Result.Failure(TaskItemErrors.TaskCannotBeCompletedByAnotherUser);
        
        // verify that it is not already completed
        if (task.CompletedAt is not null)
            return Result.Failure(TaskItemErrors.TaskAlreadyCompleted);
        
        task.CompletedAt = DateTime.UtcNow;
        task.UpdatedById = request.RequesterId;
        task.UpdatedOn = DateTime.UtcNow;

        await taskItemRepository.UpdateAsync(task, cancellationToken);
        
        return Result.Success();
    }
}