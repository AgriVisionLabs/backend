using Agrivision.Backend.Application.Features.Tasks.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Tasks.Validators;

public class UpdateTaskItemRequestValidator : AbstractValidator<UpdateTaskItemRequest>
{
    public UpdateTaskItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.AssignedToId)
            .MaximumLength(100)
            .When(x => x.AssignedToId != null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.ItemPriority)
            .IsInEnum();
    }
}