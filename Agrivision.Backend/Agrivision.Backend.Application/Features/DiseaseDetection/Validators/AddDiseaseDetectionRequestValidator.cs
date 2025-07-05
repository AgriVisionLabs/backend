using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Validators;

public class AddDiseaseDetectionRequestValidator : AbstractValidator<AddDiseaseDetectionRequest>
{
    public AddDiseaseDetectionRequestValidator()
    {
        // Ensure either image or video is provided, but not both
        RuleFor(request => request)
            .Must(HaveExactlyOneFile)
            .WithMessage("Either an image file or a video file must be provided, but not both.");

        // Image validation rules
        RuleFor(request => request.Image)
            .Must(file => file == null || file.Length > 0)
            .WithMessage("Image file cannot be empty.")
            .Must(file => file == null || 
                          (file.ContentType.Equals("image/jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                           file.ContentType.Equals("image/png", StringComparison.InvariantCultureIgnoreCase)))
            .WithMessage("Only JPEG and PNG image formats are supported.");

        // Video validation rules
        RuleFor(request => request.Video)
            .Must(file => file == null || file.Length > 0)
            .WithMessage("Video file cannot be empty.")
            .Must(file => file == null || file.ContentType.Equals("video/mp4", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("Only MP4 video format is supported.");
    }

    private static bool HaveExactlyOneFile(AddDiseaseDetectionRequest request)
    {
        var hasImage = request.Image != null;
        var hasVideo = request.Video != null;
        
        // Return true if exactly one file is provided
        return hasImage ^ hasVideo; // XOR - exactly one must be true
    }
}