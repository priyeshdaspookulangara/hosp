using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(UniCareDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateNextItemCodeAsync(string category)
        {
            string prefix = (category?.Substring(0, 3) ?? "GEN").ToUpper();
            var lastItem = await _context.InventoryItems
                                      .Where(i => i.ItemCode.StartsWith(prefix))
                                      .OrderByDescending(i => i.ItemCode)
                                      .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastItem != null)
            {
                string lastNumberStr = lastItem.ItemCode.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{prefix}{nextNumber:D4}"; // e.g., MED0001, SUP0001
        }


        public async Task<InventoryItemDto?> CreateItemAsync(CreateInventoryItemDto createDto)
        {
            var item = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ItemCode = await GenerateNextItemCodeAsync(createDto.Category),
                Name = createDto.Name,
                Description = createDto.Description,
                Category = createDto.Category,
                UnitOfMeasure = createDto.UnitOfMeasure,
                UnitPrice = createDto.UnitPrice,
                CostPrice = createDto.CostPrice,
                ReorderLevel = createDto.ReorderLevel,
                SupplierInfo = createDto.SupplierInfo,
                ExpiryDate = createDto.ExpiryDate,
                BatchNumber = createDto.BatchNumber,
                QuantityInStock = 0, // Initial quantity is always 0, must be adjusted via a transaction
                IsActive = true
            };

            try
            {
                _context.InventoryItems.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Inventory item '{item.Name}' ({item.ItemCode}) created.");
                return MapItemToDto(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating inventory item '{createDto.Name}'.");
                return null;
            }
        }

        public async Task<InventoryItemDto?> GetItemByIdAsync(Guid itemId)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            return item == null ? null : MapItemToDto(item);
        }

        public async Task<IEnumerable<InventoryItemDto>> GetAllItemsAsync()
        {
            return await _context.InventoryItems
                                 .OrderBy(i => i.Name)
                                 .Select(i => MapItemToDto(i))
                                 .ToListAsync();
        }

        public async Task<InventoryItemDto?> UpdateItemAsync(UpdateInventoryItemDto updateDto)
        {
            var item = await _context.InventoryItems.FindAsync(updateDto.Id);
            if (item == null)
            {
                _logger.LogWarning($"Inventory item with ID {updateDto.Id} not found for update.");
                return null;
            }

            // Update master data. Note: ItemCode and QuantityInStock are not updated here.
            item.Name = updateDto.Name;
            item.Description = updateDto.Description;
            item.Category = updateDto.Category;
            item.UnitOfMeasure = updateDto.UnitOfMeasure;
            item.UnitPrice = updateDto.UnitPrice;
            item.CostPrice = updateDto.CostPrice;
            item.ReorderLevel = updateDto.ReorderLevel;
            item.SupplierInfo = updateDto.SupplierInfo;
            item.ExpiryDate = updateDto.ExpiryDate;
            item.BatchNumber = updateDto.BatchNumber;
            item.IsActive = updateDto.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Inventory item '{item.Name}' ({item.ItemCode}) updated.");
                return MapItemToDto(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating inventory item ID {updateDto.Id}.");
                return null;
            }
        }

        public async Task<bool> DeleteItemAsync(Guid itemId)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item == null)
            {
                _logger.LogWarning($"Inventory item with ID {itemId} not found for soft delete.");
                return false;
            }

            // Soft delete
            item.IsActive = false;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Inventory item '{item.Name}' ({item.ItemCode}) soft deleted (deactivated).");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting inventory item ID {itemId}.");
                return false;
            }
        }

        public async Task<bool> AdjustStockAsync(Guid itemId, int quantityChanged, string reason, string? transactionType = "Adjustment")
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item == null)
            {
                _logger.LogWarning($"Inventory item with ID {itemId} not found for stock adjustment.");
                return false;
            }

            // Ensure stock doesn't go negative for deductions if that's a business rule
            if (item.QuantityInStock + quantityChanged < 0)
            {
                _logger.LogWarning($"Stock adjustment for item {item.ItemCode} failed. Insufficient stock. Current: {item.QuantityInStock}, Change: {quantityChanged}");
                return false;
            }

            var transaction = new StockTransaction
            {
                Id = Guid.NewGuid(),
                InventoryItemId = itemId,
                QuantityChanged = quantityChanged,
                TransactionType = transactionType ?? "Adjustment",
                Notes = reason,
                TransactionDate = DateTime.UtcNow
            };

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                item.QuantityInStock += quantityChanged;
                _context.StockTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation($"Stock for item {item.ItemCode} adjusted by {quantityChanged}. New quantity: {item.QuantityInStock}. Reason: {reason}");
                return true;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, $"Error adjusting stock for item ID {itemId}.");
                return false;
            }
        }


        private static InventoryItemDto MapItemToDto(InventoryItem item)
        {
            return new InventoryItemDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                UnitOfMeasure = item.UnitOfMeasure,
                QuantityInStock = item.QuantityInStock,
                UnitPrice = item.UnitPrice,
                CostPrice = item.CostPrice,
                ReorderLevel = item.ReorderLevel,
                SupplierInfo = item.SupplierInfo,
                ExpiryDate = item.ExpiryDate,
                BatchNumber = item.BatchNumber,
                IsActive = item.IsActive
            };
        }
    }
}
