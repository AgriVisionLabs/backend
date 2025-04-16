using Agrivision.Backend.Application.Features.Farm.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farm.Validators;

public class GetInvitationsRequestValidator : AbstractValidator<GetInvitationsRequest>
{
    public GetInvitationsRequestValidator()
    {
        RuleFor(req => req.FarmId)
            .NotEmpty();
    }
}