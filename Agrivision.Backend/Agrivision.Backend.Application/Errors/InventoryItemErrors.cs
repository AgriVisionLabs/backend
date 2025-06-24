using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class InventoryItemErrors
{
    public static readonly Error DuplicateName =
        new("InventoryItemErrors.DuplicateName", "Farm already have an inventory item by that name.");
    
    public static readonly Error ItemNotFound =
        new("InventoryItemErrors.ItemNotFound", "No inventory item with the specified id was found.");
    
    public static readonly Error InsufficientInventoryQuantity =
        new("InventoryItemErrors.InsufficientInventoryQuantity", "Insufficient inventory quantity to complete the transaction.");
}