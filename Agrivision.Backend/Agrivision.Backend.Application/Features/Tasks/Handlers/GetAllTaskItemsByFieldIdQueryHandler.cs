using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Application.Features.Tasks.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class GetAllTaskItemsByFieldIdQueryHandler(ITaskItemRepository taskItemRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository, IFieldRepository fieldRepository) : IRequestHandler<GetAllTaskItemsByFieldIdQuery, Result<IReadOnlyList<TaskItemResponse>>>
{
    public async Task<Result<IReadOnlyList<TaskItemResponse>>> Handle(GetAllTaskItemsByFieldIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<IReadOnlyList<TaskItemResponse>>(FieldErrors.FieldNotFound);
        
        // check if field belong to the farm 
        if (field.FarmId != request.FarmId)
            return Result.Failure <IReadOnlyList<TaskItemResponse>>(FarmErrors.UnauthorizedAction);
        
        // check if user have access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId,
                cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<TaskItemResponse>>(FarmErrors.UnauthorizedAction);
        
        // expert can't see tasks
        if (farmUserRole.FarmRole.Name is "Expert")
            return Result.Failure<IReadOnlyList<TaskItemResponse>>(FarmErrors.UnauthorizedAction);

        IReadOnlyList<TaskItem> taskItems;

        if (farmUserRole.FarmRole.Name == "Worker")
            taskItems = await taskItemRepository.GetAllByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        else
        {
            taskItems = await taskItemRepository.GetAllByFieldIdAsync(request.FieldId, cancellationToken);
        }
        
        // get all the user id so we can fetch all at once 
        var userIds = taskItems
            .SelectMany(t => new[] { t.CreatedById, t.AssignedToId, t.ClaimedById })
            .Where(id => id != null)
            .Distinct()
            .ToList();
        
        // fetch all the users using their ids and then map the id to the user's name
        var users = await userRepository.FindByIdsAsync(userIds!, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        // map to response
        var response = taskItems.Select(task => new TaskItemResponse(
            Id: task.Id,
            FarmId: field.FarmId,
            FieldId: task.FieldId,
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
            ItemPriority: task.ItemPriority
        )).ToList();

        return Result.Success<IReadOnlyList<TaskItemResponse>>(response);
    }
}