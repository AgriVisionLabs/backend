using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class ClaimTaskItemCommandHandler(ITaskItemRepository taskItemRepository, IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<ClaimTaskItemCommand, Result>
{
    public async Task<Result> Handle(ClaimTaskItemCommand request, CancellationToken cancellationToken)
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
        
        // the author can't claim the task
        if (task.CreatedById == request.RequesterId)
            return Result.Failure(TaskItemErrors.CannotAssignTasksToSelf);
        
        if (task.ClaimedById is not null)
            return Result.Failure(TaskItemErrors.AlreadyClaimed);

        task.ClaimedById = request.RequesterId;
        task.ClaimedAt = DateTime.UtcNow;
        task.UpdatedById = request.RequesterId;
        task.UpdatedOn = DateTime.UtcNow;

        await taskItemRepository.UpdateAsync(task, cancellationToken);
        
        return Result.Success();
    }
}