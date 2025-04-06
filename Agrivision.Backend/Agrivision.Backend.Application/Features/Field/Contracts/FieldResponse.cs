namespace Agrivision.Backend.Application.Features.Field.Contracts;

public record FieldResponse
(
    Guid Id,
    string Name,
    double Area,
    bool IsActive,
    Guid FarmId
);