namespace Agrivision.Backend.Application.Features.Field.Contracts;

public record UpdateFieldRequest
(
    string Name,
    double Area
);