using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FieldErrors
{
    public static readonly Error DuplicateFieldName =
        new("Field.DuplicateName", "Farm already have a field with this name.");
    public static readonly Error UnauthorizedAction = new("Field.UnauthorizedAction", "User is not authorized to perform this action.");
    public static readonly Error InvalidFieldArea =
        new("Field.InvalidArea", "Field area is invalid or exceeds farm area.");

    public static readonly Error FieldNotFound = new("Field.NotFound", "Field not found.");
}