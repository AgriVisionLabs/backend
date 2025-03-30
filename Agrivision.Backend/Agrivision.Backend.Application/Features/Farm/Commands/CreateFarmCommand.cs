using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Commands;

public record CreateFarmCommand
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    string CreatedById,
    IEnumerable<FarmMembers_Contract> FarmMembers
) : IRequest<Result<FarmResponse>>;