using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record UpdateFarmRequest
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType
);