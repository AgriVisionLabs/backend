using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farm.Validators;
public class FarmMembers_Contract_RequestValidator : AbstractValidator<FarmMembers_Contract>                                                                    
{
    public FarmMembers_Contract_RequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Role)
            .NotEmpty();
          
    }
}