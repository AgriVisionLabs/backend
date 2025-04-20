using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Commands;

public record DeleteFieldCommand
(
    Guid FarmId,
    Guid FieldId,
    string DeletedById
) : IRequest<Result>;