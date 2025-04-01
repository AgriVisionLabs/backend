using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;


namespace Agrivision.Backend.Application.Features.Farm.Commands;
public record class UpdateFarmCommand
(
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    string CreatedById,
    IEnumerable<FarmMembers_Contract> FarmMembers,
    string EncodedFarmId

) : IRequest<Result<FarmResponse>>;