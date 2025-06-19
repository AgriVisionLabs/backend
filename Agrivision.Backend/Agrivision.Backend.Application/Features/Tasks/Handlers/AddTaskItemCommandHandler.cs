using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Tasks.Commands;
using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Interfaces.Identity;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Handlers;

public class AddTaskItemCommandHandler(IFieldRepository fieldRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository, ITaskItemRepository taskItemRepository) : IRequestHandler<AddTaskItemCommand, Result<TaskItemResponse>>
{
    public async Task<Result<TaskItemResponse>> Handle(AddTaskItemCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<TaskItemResponse>(FieldErrors.FieldNotFound);
        
        // check if field is in farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<TaskItemResponse>(FarmErrors.UnauthorizedAction);
        
        // check if user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<TaskItemResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the title is already used
        if (await taskItemRepository.TitleExistsByTitleAndFieldIdAsync(request.Title, request.FieldId,
                cancellationToken))
            return Result.Failure<TaskItemResponse>(TaskItemErrors.DuplicateTitle);
        
        // check if self assigning
        if (request.AssignedToId == request.RequesterId)
            return Result.Failure<TaskItemResponse>(TaskItemErrors.CannotAssignTasksToSelf);
        
        // only owner and manager can assign tasks 
        if (farmUserRole.FarmRole.Name is "Worker" or "Expert")
            return Result.Failure<TaskItemResponse>(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if assigned user (if any) has access
        IApplicationUser? assignee = null;

        if (request.AssignedToId is not null)
        {
            var assignedTo = await farmUserRoleRepository
                .FindByUserIdAndFarmIdAsync(request.AssignedToId, request.FarmId, cancellationToken);

            if (assignedTo is null)
                return Result.Failure<TaskItemResponse>(FarmUserRoleErrors.UserRoleNotFound);
            
            
            if (assignedTo.FarmRole.Name is "Manager" && farmUserRole.FarmRole.Name == "Manager")
                return Result.Failure<TaskItemResponse>(TaskItemErrors.CannotAssignToElevatedRoles);
            

            if (assignedTo.FarmRole.Name is "Expert")
                return Result.Failure<TaskItemResponse>(TaskItemErrors.CannotAssignTasksToExpert);

            assignee = await userRepository.FindByIdAsync(request.AssignedToId);
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            AssignedToId = request.AssignedToId,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ItemPriority = request.ItemPriority,
            FieldId = request.FieldId
        };

        await taskItemRepository.AddAsync(task, cancellationToken);
        
        // get names 
        var requester = await userRepository.FindByIdAsync(request.RequesterId);
        
        // map to response
        var response = new TaskItemResponse(
            Id: task.Id,
            FarmId: request.FarmId,
            FieldId: task.FieldId,
            CreatedById: task.CreatedById,
            CreatedBy: requester!.FirstName + " " + requester.LastName,
            CreatedAt: task.CreatedOn,
            AssignedToId: task.AssignedToId,
            AssignedTo: assignee != null ? assignee.FirstName + " " + assignee.LastName : null,
            ClaimedById: task.ClaimedById,
            ClaimedBy: null,
            Title: task.Title,
            Description: task.Description,
            DueDate: task.DueDate,
            CompletedAt: task.CompletedAt,
            ItemPriority: task.ItemPriority
        );

        return Result.Success(response);
    }
}