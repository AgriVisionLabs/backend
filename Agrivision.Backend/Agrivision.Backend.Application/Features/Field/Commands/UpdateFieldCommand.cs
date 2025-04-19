using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Commands;

public record UpdateFieldCommand
(
    Guid FarmId,
    Guid FieldId,
    string Name,
    double Area,
    string UpdatedById
) : IRequest<Result>;