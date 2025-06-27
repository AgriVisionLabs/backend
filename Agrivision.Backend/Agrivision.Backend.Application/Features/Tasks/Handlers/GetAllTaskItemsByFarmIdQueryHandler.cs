using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Application.Features.Tasks.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class GetAllTaskItemsByFarmIdQueryHandler(ITaskItemRepository taskItemRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<GetAllTaskItemsByFarmIdQuery, Result<IReadOnlyList<TaskItemResponse>>>
{
    public async Task<Result<IReadOnlyList<TaskItemResponse>>> Handle(GetAllTaskItemsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<TaskItemResponse>>(FarmErrors.FarmNotFound);
        
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
            taskItems = await taskItemRepository.GetAllByFarmIdAsync(request.FarmId, cancellationToken);
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
            FarmId: farm.Id,
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
        )).ToList();

        return Result.Success<IReadOnlyList<TaskItemResponse>>(response);
    }
}