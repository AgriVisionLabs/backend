using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Fields.Contracts;

public record UpdateFieldRequest
(
    string Name,
    CropTypes Crop,
    double Area
);