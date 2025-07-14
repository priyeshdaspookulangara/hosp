using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Inventory
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly UniCareDbContext _context;
        private readonly IInventoryService _inventoryService; // For stock adjustments
        private readonly ILogger<PurchaseOrderService> _logger;
        private const string PoNumberPrefix = "PO-";

        public PurchaseOrderService(UniCareDbContext context, IInventoryService inventoryService, ILogger<PurchaseOrderService> logger)
        {
            _context = context;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        private async Task<string> GenerateNextPoNumberAsync()
        {
            var today = DateTime.Today;
            var year = today.ToString("yyyy");
            var prefixWithDate = $"{PoNumberPrefix}{year}-";

            var lastPo = await _context.PurchaseOrders
                                      .Where(po => po.PurchaseOrderNumber.StartsWith(prefixWithDate))
                                      .OrderByDescending(po => po.PurchaseOrderNumber)
                                      .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastPo != null)
            {
                string lastNumberStr = lastPo.PurchaseOrderNumber.Split('-').Last();
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{prefixWithDate}{nextNumber:D4}"; // e.g., PO-2025-0001
        }

        public async Task<PurchaseOrderDto?> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createDto)
        {
            var po = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                PurchaseOrderNumber = await GenerateNextPoNumberAsync(),
                SupplierInfo = createDto.SupplierInfo,
                ExpectedDeliveryDate = createDto.ExpectedDeliveryDate,
                Notes = createDto.Notes,
                OrderDate = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Pending,
                Items = createDto.Items.Select(itemDto => new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    InventoryItemId = itemDto.InventoryItemId,
                    QuantityOrdered = itemDto.QuantityOrdered,
                    UnitPrice = itemDto.UnitPrice,
                    QuantityReceived = 0
                }).ToList()
            };

            po.TotalAmount = po.Items.Sum(item => item.QuantityOrdered * item.UnitPrice);

            try
            {
                _context.PurchaseOrders.Add(po);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Purchase Order {po.PurchaseOrderNumber} created.");
                return await GetPurchaseOrderByIdAsync(po.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order.");
                return null;
            }
        }

        public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(Guid poId)
        {
            var po = await _context.PurchaseOrders
                                   .Include(p => p.Items)
                                   .ThenInclude(i => i.InventoryItem)
                                   .FirstOrDefaultAsync(p => p.Id == poId);

            return po == null ? null : MapPoToDto(po);
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
        {
            var pos = await _context.PurchaseOrders
                                    .OrderByDescending(p => p.OrderDate)
                                    .ToListAsync();
            return pos.Select(MapPoToDto);
        }

        public async Task<bool> ApprovePurchaseOrderAsync(Guid poId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null || po.Status != PurchaseOrderStatus.Pending)
            {
                _logger.LogWarning($"PO {poId} not found or not in Pending state for approval.");
                return false;
            }

            po.Status = PurchaseOrderStatus.Approved;
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"PO {po.PurchaseOrderNumber} approved.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving PO {poId}.");
                return false;
            }
        }

        public async Task<bool> CancelPurchaseOrderAsync(Guid poId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null || po.Status == PurchaseOrderStatus.Completed || po.Status == PurchaseOrderStatus.PartiallyReceived)
            {
                _logger.LogWarning($"PO {poId} cannot be cancelled in its current state: {po?.Status}.");
                return false;
            }

            po.Status = PurchaseOrderStatus.Cancelled;
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"PO {po.PurchaseOrderNumber} cancelled.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling PO {poId}.");
                return false;
            }
        }

        public async Task<PurchaseOrderDto?> ReceiveGoodsAsync(Guid poId, List<ReceivedItemDto> receivedItems)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = await _context.PurchaseOrders
                                       .Include(p => p.Items)
                                       .FirstOrDefaultAsync(p => p.Id == poId);

                if (po == null || po.Status != PurchaseOrderStatus.Approved)
                {
                    _logger.LogWarning($"PO {poId} not found or not in Approved state for receiving goods.");
                    await transaction.RollbackAsync();
                    return null;
                }

                foreach (var receivedItem in receivedItems)
                {
                    var poItem = po.Items.FirstOrDefault(i => i.Id == receivedItem.PurchaseOrderItemId);
                    if (poItem == null) throw new InvalidOperationException($"PO Item {receivedItem.PurchaseOrderItemId} not found on PO {poId}.");

                    if (poItem.QuantityReceived + receivedItem.QuantityReceived > poItem.QuantityOrdered)
                    {
                        throw new InvalidOperationException($"Cannot receive more items than ordered for {poItem.InventoryItemId}.");
                    }

                    // Adjust stock using InventoryService
                    var stockAdjusted = await _inventoryService.AdjustStockAsync(
                        poItem.InventoryItemId,
                        receivedItem.QuantityReceived,
                        $"Goods received against PO {po.PurchaseOrderNumber}",
                        "Purchase"
                    );

                    if (!stockAdjusted)
                    {
                        // This should ideally not fail if logic is correct, but as a safeguard
                        throw new InvalidOperationException($"Failed to adjust stock for item {poItem.InventoryItemId}.");
                    }

                    poItem.QuantityReceived += receivedItem.QuantityReceived;
                }

                // Update PO status
                bool allItemsReceived = po.Items.All(i => i.QuantityReceived >= i.QuantityOrdered);
                po.Status = allItemsReceived ? PurchaseOrderStatus.Completed : PurchaseOrderStatus.PartiallyReceived;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Goods received for PO {po.PurchaseOrderNumber}. New status: {po.Status}.");
                return await GetPurchaseOrderByIdAsync(po.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error receiving goods for PO {poId}.");
                return null;
            }
        }

        private static PurchaseOrderDto MapPoToDto(PurchaseOrder po)
        {
            return new PurchaseOrderDto
            {
                Id = po.Id,
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                SupplierInfo = po.SupplierInfo,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                Notes = po.Notes,
                Items = po.Items?.Select(i => new PurchaseOrderItemDto
                {
                    Id = i.Id,
                    InventoryItemId = i.InventoryItemId,
                    ItemName = i.InventoryItem?.Name ?? "N/A",
                    ItemCode = i.InventoryItem?.ItemCode ?? "N/A",
                    QuantityOrdered = i.QuantityOrdered,
                    UnitPrice = i.UnitPrice,
                    QuantityReceived = i.QuantityReceived
                }).ToList() ?? new List<PurchaseOrderItemDto>()
            };
        }
    }
}
