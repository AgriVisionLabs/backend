using Agrivision.Backend.Application.Features.Field.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Commands;

public record CreateFieldCommand
(
    string Name,
    double Area,
    Guid FarmId,
    string CreatedById
) : IRequest<Result<FieldResponse>>;