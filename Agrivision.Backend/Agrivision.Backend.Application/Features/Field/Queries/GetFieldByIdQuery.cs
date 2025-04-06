using Agrivision.Backend.Application.Features.Field.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Queries;

public record GetFieldByIdQuery
(
    Guid FieldId
) : IRequest<Result<FieldResponse>>;