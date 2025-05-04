

using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
public record DetectionResponse
(
    DetecionResults Status ,
    string ImagePath,
    DateTime CreatedOn ,
    string CreatedById 
);