using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farm.Validators;
public class CreateFarm_FarmMembers_RequestValidator : AbstractValidator<CreateFarm_FarmMembers>                                                                    
{
    public CreateFarm_FarmMembers_RequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Role)
            .NotEmpty();
          
    }
}