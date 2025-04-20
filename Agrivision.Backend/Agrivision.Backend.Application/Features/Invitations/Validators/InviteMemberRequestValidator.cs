using Agrivision.Backend.Application.Features.Invitations.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Invitations.Validators;

public class InviteMemberRequestValidator : AbstractValidator<InviteMemberRequest>
{
    public InviteMemberRequestValidator()
    {
        RuleFor(req => req.Recipient)
            .NotEmpty()
            .WithMessage("Recipient is required.")
            .MaximumLength(150)
            .WithMessage("Recipient must not exceed 150 characters.");
        
        When(req => req.Recipient.Contains('@'), () =>
        {
            RuleFor(req => req.Recipient)
                .EmailAddress()
                .WithMessage("Invalid email format.");
        });

        RuleFor(req => req.RoleName)
            .NotEmpty();
    }
}