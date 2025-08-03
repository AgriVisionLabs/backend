using Agrivision.Backend.Application.Features.Account.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Account.Validators;

public class PfpRequestValidator : AbstractValidator<PfpRequest>
{
    public PfpRequestValidator()
    {
        RuleFor(request => request.Image)
            .Must(file => file == null || file.Length > 0)
            .WithMessage("Image file cannot be empty.")
            .Must(file => file == null || 
                          (file.ContentType.Equals("image/jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                           file.ContentType.Equals("image/png", StringComparison.InvariantCultureIgnoreCase)))
            .WithMessage("Only JPEG and PNG image formats are supported.");
    }
}