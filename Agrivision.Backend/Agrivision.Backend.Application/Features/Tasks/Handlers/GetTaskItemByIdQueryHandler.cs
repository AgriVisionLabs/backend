using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Application.Features.Tasks.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class GetTaskItemByIdQueryHandler(ITaskItemRepository taskItemRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<GetTaskItemByIdQuery, Result<TaskItemResponse>>
{
    public async Task<Result<TaskItemResponse>> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<TaskItemResponse>(FarmErrors.FarmNotFound);
        
        // check if user have access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<TaskItemResponse>(FarmErrors.UnauthorizedAction);
        
        // expert can't see tasks
        if (farmUserRole.FarmRole.Name is "Expert")
            return Result.Failure<TaskItemResponse>(FarmErrors.UnauthorizedAction);
        
        // get the task 
        var task = await taskItemRepository.FindByIdAsync(request.TaskItemId, cancellationToken);
        if (task is null)
            return Result.Failure<TaskItemResponse>(TaskItemErrors.TaskItemNotFound);

        // check if the task belong to the farm
        if (task.Field.FarmId != request.FarmId)
            return Result.Failure<TaskItemResponse>(TaskItemErrors.TaskItemNotFound);
        
        if (farmUserRole.FarmRole.Name == "Worker")
        {
            var isUnclaimedAndUnassigned = task.ClaimedById is null && task.AssignedToId is null;
            var isOwnedByRequester = task.AssignedToId == request.RequesterId || task.ClaimedById == request.RequesterId;

            if (!isUnclaimedAndUnassigned && !isOwnedByRequester)
                return Result.Failure<TaskItemResponse>(TaskItemErrors.TaskItemNotFound);
        }
        
        // fetch the user
        var userIds = new[] { task.CreatedById, task.AssignedToId, task.ClaimedById }
            .Where(id => id is not null)
            .Distinct()
            .ToList();
        
        var users = await userRepository.FindByIdsAsync(userIds!, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");
        
        // map to response
        var response = new TaskItemResponse(
            Id: task.Id,
            FarmId: task.Field.FarmId,
            FieldId: task.FieldId,
            FieldName: task.Field.Name,
            CreatedById: task.CreatedById,
            CreatedBy: userMap.GetValueOrDefault(task.CreatedById, "Unknown"),
            CreatedAt: task.CreatedOn,
            AssignedToId: task.AssignedToId,
            AssignedTo: task.AssignedToId is not null ? userMap.GetValueOrDefault(task.AssignedToId, "Unknown") : null,
            AssignedAt: task.AssignedAt,
            ClaimedById: task.ClaimedById,
            ClaimedBy: task.ClaimedById is not null ? userMap.GetValueOrDefault(task.ClaimedById, "Unknown") : null,
            ClaimedAt: task.ClaimedAt,
            Title: task.Title,
            Description: task.Description,
            DueDate: task.DueDate,
            CompletedAt: task.CompletedAt,
            ItemPriority: task.ItemPriority,
            Category: task.Category
        );
        
        return Result.Success(response);
    }
}