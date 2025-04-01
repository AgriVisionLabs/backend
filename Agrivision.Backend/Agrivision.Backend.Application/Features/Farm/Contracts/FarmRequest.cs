using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record FarmRequest
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    IEnumerable<FarmMembers_Contract> FarmMembers
);