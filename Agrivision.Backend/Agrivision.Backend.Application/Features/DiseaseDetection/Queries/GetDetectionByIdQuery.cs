using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Queries;

public record GetDetectionByIdQuery
(
    Guid FarmId,
    Guid DetectionId,
    string RequesterId
) : IRequest<Result<DiseaseDetectionResponse>>;