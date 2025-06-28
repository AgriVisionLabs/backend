using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Queries;

public record GetFieldByIdQuery
(
    string RequesterId,
    Guid FarmId,
    Guid FieldId
) : IRequest<Result<FieldResponse>>;