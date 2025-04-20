namespace Agrivision.Backend.Application.Features.Fields.Contracts;

public record CreateFieldRequest
(
    string Name,
    double Area
);