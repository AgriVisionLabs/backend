using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farms.Contracts;

public record UpdateFarmRequest
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType
);