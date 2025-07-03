using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Validators;

public class AddDiseaseDetectionRequestValidator : AbstractValidator<AddDiseaseDetectionRequest>
{
    public AddDiseaseDetectionRequestValidator()
    {
        RuleFor(request => request.Image)
            .Cascade(CascadeMode.Stop) // Ensures subsequent rules stop if NotNull fails
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Image file cannot be empty.")
            .Must(file => file != null && 
                          (file.ContentType.ToLowerInvariant() == "image/jpeg" ||
                           file.ContentType.ToLowerInvariant() == "image/png"))
            .WithMessage("Only JPEG and PNG image formats are supported.");
    }
}