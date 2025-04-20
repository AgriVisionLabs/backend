namespace Agrivision.Backend.Application.Features.Fields.Contracts;

public record FieldResponse
(
    Guid Id,
    string Name,
    double Area,
    bool IsActive,
    Guid FarmId
);