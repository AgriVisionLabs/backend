using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using MediatR;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Queries;

public record GetAllDetectionByFieldIdQuery
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId
) : IRequest<IReadOnlyList<DiseaseDetectionResponse>>;