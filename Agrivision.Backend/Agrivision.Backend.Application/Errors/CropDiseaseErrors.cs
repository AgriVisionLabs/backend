using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class CropDiseaseErrors
{
    public static readonly Error CropDiseaseNotFound = new("CropDisease.NotFound", "Crop disease was not found.");
}