using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Inventory
{
    public class SaleService : ISaleService
    {
        private readonly UniCareDbContext _context;
        private readonly IInventoryService _inventoryService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<SaleService> _logger;
        private const string SaleNumberPrefix = "SALE-";

        public SaleService(
            UniCareDbContext context,
            IInventoryService inventoryService,
            IInvoiceService invoiceService,
            ILogger<SaleService> logger)
        {
            _context = context;
            _inventoryService = inventoryService;
            _invoiceService = invoiceService;
            _logger = logger;
        }

        private async Task<string> GenerateNextSaleNumberAsync()
        {
            var today = DateTime.Today;
            var yearMonthDay = today.ToString("yyyyMMdd");
            var prefixWithDate = $"{SaleNumberPrefix}{yearMonthDay}-";

            var lastSale = await _context.Sales
                                      .Where(s => s.SaleNumber.StartsWith(prefixWithDate))
                                      .OrderByDescending(s => s.SaleNumber)
                                      .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastSale != null)
            {
                string lastNumberStr = lastSale.SaleNumber.Split('-').Last();
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{prefixWithDate}{nextNumber:D4}"; // e.g., SALE-20250710-0001
        }

        public async Task<SaleDto?> CreateSaleAsync(CreateSaleDto createDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Create the Sale entity
                var sale = new Sale
                {
                    Id = Guid.NewGuid(),
                    SaleNumber = await GenerateNextSaleNumberAsync(),
                    PatientId = createDto.PatientId,
                    SaleDate = DateTime.UtcNow,
                    PaymentMethod = createDto.PaymentMethod,
                    Notes = createDto.Notes,
                    Items = new List<SaleItem>()
                };

                // Step 2: Process each item, adjust stock, and build SaleItem entities
                foreach (var itemDto in createDto.Items)
                {
                    // Decrease stock via InventoryService. This checks for sufficient stock.
                    var stockAdjusted = await _inventoryService.AdjustStockAsync(
                        itemDto.InventoryItemId,
                        -itemDto.Quantity, // Negative quantity for deduction
                        $"Sale {sale.SaleNumber}",
                        "Sale"
                    );

                    if (!stockAdjusted)
                    {
                        // If stock adjustment fails, roll back everything and fail the sale
                        await transaction.RollbackAsync();
                        _logger.LogWarning($"Sale failed due to insufficient stock for item ID {itemDto.InventoryItemId}.");
                        return null;
                    }

                    var saleItem = new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        InventoryItemId = itemDto.InventoryItemId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice
                    };
                    sale.Items.Add(saleItem);
                }

                // Step 3: Calculate total and save the Sale
                sale.TotalAmount = sale.Items.Sum(item => item.TotalPrice);
                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Step 4: If payment method is "AddToPatientBill", create an invoice
                if (sale.PaymentMethod == PaymentMethod.AddToPatientBill)
                {
                    if (!sale.PatientId.HasValue)
                    {
                        throw new InvalidOperationException("Cannot add to patient bill without a patient ID.");
                    }

                    var createInvoiceDto = new CreateInvoiceDto
                    {
                        PatientId = sale.PatientId.Value,
                        InvoiceDate = sale.SaleDate.Date,
                        DueDate = sale.SaleDate.Date.AddDays(30),
                        InvoiceItems = sale.Items.Select(si => new CreateInvoiceItemDto
                        {
                            Description = $"Medical Shop Sale: {(_context.InventoryItems.Find(si.InventoryItemId)?.Name ?? "Item")}",
                            Quantity = si.Quantity,
                            UnitPrice = si.UnitPrice
                        }).ToList()
                    };

                    var invoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto);
                    if (invoice == null)
                    {
                        // If invoice creation fails, the whole sale should be rolled back.
                        throw new InvalidOperationException("Failed to create invoice for the patient bill.");
                    }
                     _logger.LogInformation($"Invoice {invoice.InvoiceNumber} created for sale {sale.SaleNumber}.");
                }


                // Step 5: Commit transaction
                await transaction.CommitAsync();
                _logger.LogInformation($"Sale {sale.SaleNumber} created successfully.");
                return await GetSaleByIdAsync(sale.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating sale.");
                return null;
            }
        }

        public async Task<SaleDto?> GetSaleByIdAsync(Guid saleId)
        {
            var sale = await _context.Sales
                                     .Include(s => s.Patient)
                                     .Include(s => s.Items)
                                     .ThenInclude(i => i.InventoryItem)
                                     .FirstOrDefaultAsync(s => s.Id == saleId);

            return sale == null ? null : MapSaleToDto(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _context.Sales
                                      .Include(s => s.Patient)
                                      .OrderByDescending(s => s.SaleDate)
                                      .ToListAsync();
            return sales.Select(MapSaleToDto);
        }

        private static SaleDto MapSaleToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                PatientId = sale.PatientId,
                PatientName = sale.Patient != null ? $"{sale.Patient.FirstName} {sale.Patient.LastName}".Trim() : "Walk-in Customer",
                TotalAmount = sale.TotalAmount,
                PaymentMethod = sale.PaymentMethod,
                Notes = sale.Notes,
                Items = sale.Items?.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    InventoryItemId = i.InventoryItemId,
                    ItemName = i.InventoryItem?.Name ?? "N/A",
                    ItemCode = i.InventoryItem?.ItemCode ?? "N/A",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList() ?? new List<SaleItemDto>()
            };
        }
    }
}
