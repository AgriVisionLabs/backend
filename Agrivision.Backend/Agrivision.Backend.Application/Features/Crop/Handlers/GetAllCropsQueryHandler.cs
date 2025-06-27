using Agrivision.Backend.Application.Features.Crop.Contracts;
using Agrivision.Backend.Application.Features.Crop.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Crop.Handlers;

public class GetAllCropsQueryHandler(ICropRepository cropRepository) : IRequestHandler<GetAllCropsQuery, Result<IReadOnlyList<CropResponse>>>
{
    public async Task<Result<IReadOnlyList<CropResponse>>> Handle(GetAllCropsQuery request, CancellationToken cancellationToken)
    {
        var crops = await cropRepository.GetAllAsync(cancellationToken);
        
        // map to response 
        int currentMonth = DateTime.UtcNow.Month; // or use DateTime.Now if you're local

        var cropResponses = crops.Select(crop => new CropResponse(
            crop.Id,
            crop.Name,
            crop.CropType,
            crop.SoilType,
            crop.SupportsDiseaseDetection,
            crop.PlantingMonths.Contains(currentMonth)
        )).ToList();

        return Result.Success<IReadOnlyList<CropResponse>>(cropResponses);
    }
}