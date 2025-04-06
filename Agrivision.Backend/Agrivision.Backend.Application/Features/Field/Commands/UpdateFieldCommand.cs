using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Commands;

public record UpdateFieldCommand
(
    Guid Id,
    string Name,
    double Area,
    string UpdatedById
) : IRequest<Result>;