namespace Agrivision.Backend.Application.Features.Field.Contracts;

public record CreateFieldRequest
(
    string Name,
    double Area,
    Guid FarmId
);