using Agrivision.Backend.Application.Features.Account.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Account.Validators;

public class UpdateNotificationPreferenceRequestValidator : AbstractValidator<UpdateNotificationPreferenceRequest>
{
    public UpdateNotificationPreferenceRequestValidator()
    {
        RuleFor(r => r.IsEnabled)
            .NotNull();

        RuleFor(r => r.Irrigation)
            .NotNull();
        
        RuleFor(r => r.Task)
            .NotNull();
        
        RuleFor(r => r.Message)
            .NotNull();
        
        RuleFor(r => r.Alert)
            .NotNull();
        
        RuleFor(r => r.Warning)
            .NotNull();
        
        RuleFor(r => r.SystemUpdate)
            .NotNull();
    }
}