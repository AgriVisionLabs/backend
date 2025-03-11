using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record CreateFarmRequest
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    IEnumerable <CreateFarm_FarmMembers> FarmMembers
);