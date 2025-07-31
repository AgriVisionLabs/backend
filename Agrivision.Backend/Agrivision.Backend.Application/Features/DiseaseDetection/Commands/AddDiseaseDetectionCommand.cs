using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Commands;

public class AddDiseaseDetectionCommand : IRequest<Result<DiseaseDetectionResponse>>
{
    public Guid FarmId { get; set; }
    public Guid FieldId { get; set; }
    public string RequesterId { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public IFormFile? Video { get; set; }
}