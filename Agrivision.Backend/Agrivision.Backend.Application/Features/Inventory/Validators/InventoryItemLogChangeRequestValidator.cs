using Agrivision.Backend.Application.Features.Inventory.Contracts;
using Agrivision.Backend.Domain.Enums.Core;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Inventory.Validators;

public class InventoryItemLogChangeRequestValidator : AbstractValidator<InventoryItemLogChangeRequest>
{
    public InventoryItemLogChangeRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .Must(q => q != 0)
            .WithMessage("Quantity must not be zero.");
        
        RuleFor(x => x.Reason)
            .IsInEnum()
            .WithMessage("Invalid reason for inventory change.");
        
        RuleFor(x => x.Reason)
            .Must(reason => reason != InventoryTransactionType.ManualAdjustment)
            .WithMessage("Manual adjustments are not allowed through this endpoint.");
    }
}