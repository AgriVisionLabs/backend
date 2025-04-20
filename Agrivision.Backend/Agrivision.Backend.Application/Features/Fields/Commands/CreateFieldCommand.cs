using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Commands;

public record CreateFieldCommand
(
    string Name,
    double Area,
    Guid FarmId,
    string CreatedById
) : IRequest<Result<FieldResponse>>;