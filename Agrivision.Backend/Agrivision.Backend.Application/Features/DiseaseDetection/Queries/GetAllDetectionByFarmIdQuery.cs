using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Queries;

public record GetAllDetectionByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<DiseaseDetectionResponse>>>;