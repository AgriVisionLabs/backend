using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farms.Contracts;

public record CreateFarmRequest
(
    string Name,
    double Area,
    string Location,
    SoilType SoilType
);