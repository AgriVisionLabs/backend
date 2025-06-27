using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Commands;

public record UpdateFieldCommand
(
    Guid FarmId,
    Guid FieldId,
    string Name,
    double Area,
    string UpdatedById
) : IRequest<Result>;