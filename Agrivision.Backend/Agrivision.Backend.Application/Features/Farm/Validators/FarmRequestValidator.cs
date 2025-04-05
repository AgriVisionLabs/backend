using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farm.Validators;

public class FarmRequestValidator : AbstractValidator<FarmRequest>
{
    public FarmRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(request => request.Area)
            .NotEmpty()
            .GreaterThanOrEqualTo(0.25)
            .WithMessage("The farm area must be at least 0.25 acres.");

        RuleFor(request => request.Location)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(request => request.SoilType)
            .NotEmpty();
       
        RuleForEach(x => x.FarmMembers)
                   .SetInheritanceValidator(v => v.Add(new FarmMembers_Contract_RequestValidator()));

        RuleFor(x => x.FarmMembers)
            .Must(HaveUniqueEmails).WithMessage("FarmMembers cannot contain duplicate emails.");
    }

    private bool HaveUniqueEmails(IEnumerable<FarmMembers_Contract> farmMembers)
    {
        if (farmMembers == null) return true; 
        var emails = farmMembers.Select(m => m.Email).ToList();
        return emails.Count == emails.Distinct().Count();
    }
}