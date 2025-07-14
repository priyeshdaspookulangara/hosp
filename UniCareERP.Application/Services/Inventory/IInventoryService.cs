using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;

namespace UniCareERP.Application.Services.Inventory
{
    public interface IInventoryService
    {
        // Item Master CRUD
        Task<InventoryItemDto?> CreateItemAsync(CreateInventoryItemDto createDto);
        Task<InventoryItemDto?> GetItemByIdAsync(Guid itemId);
        Task<IEnumerable<InventoryItemDto>> GetAllItemsAsync();
        Task<InventoryItemDto?> UpdateItemAsync(UpdateInventoryItemDto updateDto);
        Task<bool> DeleteItemAsync(Guid itemId); // Soft delete

        // Stock Management
        Task<bool> AdjustStockAsync(Guid itemId, int quantityChanged, string reason, string? transactionType = "Adjustment");
    }
}
