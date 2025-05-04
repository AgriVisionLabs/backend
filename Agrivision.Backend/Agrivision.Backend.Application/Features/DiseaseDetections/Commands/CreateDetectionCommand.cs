using Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.DiseaseDetections.Commands;
public record CreateDetectionCommand
(
 IFormFile Image,
 Guid FarmId,
 Guid FeildId,
 string CreatedById

) : IRequest<Result<List<DetectionResponse>>>;