using Agrivision.Backend.Application.Features.DiseaseDetections.Commands;
using Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.DetectionModel;
using Agrivision.Backend.Application.Services.FileManagement;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using System.Text.Json;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using System.Collections.Generic;
using Mapster;

namespace Agrivision.Backend.Application.Features.DiseaseDetections.Handlers;
public class CreateDetectionCommandHandler(IFileService fileService,
                                           IDiseaseDetectionService diseaseDetectionService,
                                           IFieldRepository fieldRepository,
                                           IDiseaseRepository diseaseRepository,
                                           IDiseaseDetectionRepository diseaseDetectionRepository) 
                                                                             : IRequestHandler<CreateDetectionCommand, Result<List<DetectionResponse>>>
{
    public async Task<Result<List<DetectionResponse>>> Handle(CreateDetectionCommand request, CancellationToken cancellationToken)
    {
       //check if crop supported to be proccessd
        var feild=await fieldRepository.FindByIdAsync(request.FeildId, cancellationToken);
        var canBeProccessed = feild!.CropType.SupportsDiseaseDetection;
        if (!canBeProccessed)
            return Result.Failure< List < DetectionResponse >> (DiseaseDetectionErrors.CropNotSupported);

        
        
        var imagePath = await fileService.UploadImageAsync(request.Image,cancellationToken);

        
        var ModelPredict=await diseaseDetectionService.NewDetectionAsync(imagePath);


        // Deserialize JSON response
        var detectionResult = JsonSerializer.Deserialize<ModelResponseContent>(ModelPredict);
        if(detectionResult.predictions is null)
            return Result.Failure< List < DetectionResponse >> (DiseaseDetectionErrors.InappropriateImage);

        List<DiseaseDetection> DetectedDiseases = [];
        
        foreach (var prediction in detectionResult.predictions)
        {
            var disease = await diseaseRepository.GetByClassId(prediction.ClassId, cancellationToken);
            if(disease is null|| disease.CropTypeId!=feild.CropTypeId)
                return Result.Failure< List < DetectionResponse >> (DiseaseDetectionErrors.ImageForAnotherCrop);

            var detectionObject = new DiseaseDetection
            {
                FarmId = request.FarmId,
                FieldId = request.FeildId,
                CreatedById = request.CreatedById,
                ImagePath = imagePath,
                DiseaseId=disease.Id,
                Status= disease.Is_Safe? DetecionResults.Healthy: 
                                                                  prediction.Confidence>0.70? DetecionResults.Infected:DetecionResults.At_Risk
            };
            DetectedDiseases.Add(detectionObject);
        }
        await diseaseDetectionRepository.AddRange(DetectedDiseases, cancellationToken);
        var response = DetectedDiseases.Adapt<List<DetectionResponse>>();

        return Result.Success( response);

    }
}
