using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class DiseaseDetectionErrors
{
    public static readonly Error DiseaseDetectionNotAllowedInEmptyField = new("DiseaseDetection.EmptyField", "Cannot perform disease detection in a field with no planted crop.");
    
    public static readonly Error CropNotSupportedForDiseaseDetection = new(
        "DiseaseDetection.UnsupportedCrop",
        "The currently planted crop does not support disease detection."
    );
    
    public static readonly Error ConfidenceLabelMismatch = new(
        "DiseaseDetection.ConfidenceLabelMismatch",
        "Confidence label mismatch."
    );
}