using Agrivision.Backend.Application.Features.Inventory.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Inventory.Validators;

public class UpdateInventoryItemRequestValidator : AbstractValidator<UpdateInventoryItemRequest>
{
    public UpdateInventoryItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(100).WithMessage("Item name must be 100 characters or fewer.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid category selected.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be zero or positive.");

        RuleFor(x => x.ThresholdQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Threshold quantity must be zero or positive.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost must be zero or positive.");

        RuleFor(x => x.MeasurementUnit)
            .NotEmpty().WithMessage("Measurement unit is required.")
            .MaximumLength(20).WithMessage("Measurement unit must be 20 characters or fewer.");

        RuleFor(x => x.ExpirationDate)
            .Must(date => date == null || date > DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future if provided.");
    }
}