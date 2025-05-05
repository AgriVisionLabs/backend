using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Domain.Enums.Core;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.AutomationRules.Validators;

public class UpdateAutomationRuleRequestValidator : AbstractValidator<UpdateAutomationRuleRequest>
{
    public UpdateAutomationRuleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rule name is required.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Automation rule type must be specified.");

        When(x => x.Type == AutomationRuleType.Threshold, () =>
        {
            RuleFor(x => x.TargetSensorType)
                .NotNull().WithMessage("Target sensor type is required for threshold-based rules.");

            RuleFor(x => x.MinThresholdValue)
                .NotNull().WithMessage("Min threshold value is required for threshold-based rules.");

            RuleFor(x => x.StartTime)
                .Null().WithMessage("Start time must be null for threshold-based rules.");

            RuleFor(x => x.EndTime)
                .Null().WithMessage("End time must be null for threshold-based rules.");

            RuleFor(x => x.ActiveDays)
                .Null().WithMessage("Active days must be null for threshold-based rules.");
        });

        When(x => x.Type == AutomationRuleType.Scheduled, () =>
        {
            RuleFor(x => x.StartTime)
                .NotNull().WithMessage("Start time is required for scheduled rules.");

            RuleFor(x => x.EndTime)
                .NotNull().WithMessage("End time is required for scheduled rules.");

            RuleFor(x => x.ActiveDays)
                .NotNull().WithMessage("Active days are required for scheduled rules.");

            RuleFor(x => x.TargetSensorType)
                .Null().WithMessage("Target sensor type must be null for scheduled rules.");

            RuleFor(x => x.MinThresholdValue)
                .Null().WithMessage("Min threshold value must be null for scheduled rules.");

            RuleFor(x => x.MaxThresholdValue)
                .Null().WithMessage("Max threshold value must be null for scheduled rules.");

            RuleFor(x => x)
                .Must(x => x.StartTime <= x.EndTime)
                .WithMessage("Start time must be before or equal to end time.")
                .When(x => x.StartTime.HasValue && x.EndTime.HasValue);
        });
    }
}