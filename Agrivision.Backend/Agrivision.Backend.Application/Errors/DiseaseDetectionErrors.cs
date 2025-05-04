

using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;
public static class DiseaseDetectionErrors
{
    public static readonly Error CropNotSupported = new("DiseaseDetection.CropNotSupported", "This crop is not supported for disease detection yet.");

    public static readonly Error InappropriateImage = new("DiseaseDetection.InappropriateImage", "Please upload a clear iamge for the leaf .");

    public static readonly Error ImageForAnotherCrop = new("DiseaseDetection.ImageForAnotherCrop", "The uploaded image for another crop,Not the crop of this field.");

}
