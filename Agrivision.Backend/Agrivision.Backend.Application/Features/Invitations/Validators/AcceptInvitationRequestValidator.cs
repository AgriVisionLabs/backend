using Agrivision.Backend.Application.Features.Invitations.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Invitations.Validators;

public class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
{
    public AcceptInvitationRequestValidator()
    {
        RuleFor(req => req.Token)
            .NotEmpty();
    }
}