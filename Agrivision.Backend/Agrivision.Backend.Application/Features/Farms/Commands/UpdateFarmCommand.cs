using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Commands;

public record UpdateFarmCommand
(
    Guid Id,
    string Name,
    double Area,
    string Location,
    SoilType SoilType,
    string UpdatedById
) : IRequest<Result>;