using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Commands;

public record DeleteFarmCommand
(
    Guid Id,
    string DeletedById
) : IRequest<Result>;