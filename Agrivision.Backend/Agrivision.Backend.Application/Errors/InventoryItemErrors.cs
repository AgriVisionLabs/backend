using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class InventoryItemErrors
{
    public static readonly Error DuplicateName =
        new("InventoryItemErrors.DuplicateName", "Farm already have an inventory item by that name.");
}