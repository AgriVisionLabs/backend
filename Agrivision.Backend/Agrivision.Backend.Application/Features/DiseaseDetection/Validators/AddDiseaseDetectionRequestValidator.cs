using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Validators;

public class AddDiseaseDetectionRequestValidator : AbstractValidator<AddDiseaseDetectionRequest>
{
    public AddDiseaseDetectionRequestValidator()
    {
        RuleFor(request => request.Image)
            .NotNull().WithMessage("Image file is required.")
            .Must(file => file.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(file =>
            {
                var contentType = file.ContentType.ToLowerInvariant();
                return contentType == "image/jpeg" || contentType == "image/png";
            }).WithMessage("Only JPEG and PNG image formats are supported.");
        
        RuleFor(request => request.Image)
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Please upload a non-empty file.");
    }
}