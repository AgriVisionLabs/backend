using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Commands;

public record DeleteFieldCommand
(
    Guid Id,
    string DeletedById
) : IRequest<Result>;