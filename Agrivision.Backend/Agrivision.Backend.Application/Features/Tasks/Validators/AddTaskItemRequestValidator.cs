using Agrivision.Backend.Application.Features.Tasks.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Tasks.Validators;

public class AddTaskItemRequestValidator : AbstractValidator<AddTaskItemRequest>
{
    public AddTaskItemRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(request => request.ItemPriority)
            .IsInEnum()
            .WithMessage("Invalid priority value.");
        
        RuleFor(request => request.Category)
            .IsInEnum()
            .WithMessage("Invalid category value.");

        RuleFor(request => request.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(request => request.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}