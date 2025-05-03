using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Farms.Contracts;

public record FarmResponse
(
    Guid FarmId,
    string Name,
    double Area,
    string Location,
    SoilTypes SoilType,
    int FieldsNo,
    string RoleName,
    string OwnerId,
    bool IsOwner
);