namespace Agrivision.Backend.Application.Features.Fields.Contracts;

public record UpdateFieldRequest
(
    string Name,
    double Area
);