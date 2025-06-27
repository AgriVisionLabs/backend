using Agrivision.Backend.Application.Features.Farms.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Commands;

public record CreateFarmCommand
(
    string Name,
    double Area,
    string Location,
    SoilType SoilType,
    string CreatedById
) : IRequest<Result<FarmResponse>>;