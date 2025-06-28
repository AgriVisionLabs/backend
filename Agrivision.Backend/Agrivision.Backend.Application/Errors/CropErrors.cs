using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class CropErrors
{
    public static readonly Error CropNotFound = new("Crop.NotFound", "The requested crop was not found.");
}