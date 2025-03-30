using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record FarmResponse
(
    string Id,
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    string OwnerId,
    List<FarmMembers_Contract> FarmMembers

);