using Agrivision.Backend.Application.Features.Members.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Members.Validators;

public class UpdateMemberRoleRequestValidator : AbstractValidator<UpdateMemberRoleRequest>
{
    public UpdateMemberRoleRequestValidator()
    {
        RuleFor(req => req.RoleName)
            .NotEmpty();
    }
}