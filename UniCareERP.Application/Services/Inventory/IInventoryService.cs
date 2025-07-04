using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Inventory; // Assuming DTOs will be created later

namespace UniCareERP.Application.Services.Inventory
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync(); // Replace with Dto later
        Task<InventoryItem?> GetInventoryItemByIdAsync(Guid id);       // Replace with Dto later
        // Add other method signatures (e.g., for Stock Transactions)
    }
}
