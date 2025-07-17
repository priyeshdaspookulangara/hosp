using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class InvoiceService : IInvoiceService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IGeneralLedgerService _generalLedgerService;
        private const string InvoiceNumberPrefix = "INV-";

        public InvoiceService(
            UniCareDbContext context,
            ILogger<InvoiceService> logger,
            IPaymentService paymentService,
            IGeneralLedgerService generalLedgerService)
        {
            _context = context;
            _logger = logger;
            _paymentService = paymentService;
            _generalLedgerService = generalLedgerService;
        }

        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            var today = DateTime.Today;
            var yearMonth = today.ToString("yyyy-MM");
            var prefixWithDate = $"{InvoiceNumberPrefix}{yearMonth}-";

            var lastInvoice = await _context.Invoices
                                      .Where(i => i.InvoiceNumber.StartsWith(prefixWithDate))
                                      .OrderByDescending(i => i.InvoiceNumber)
                                      .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                string lastNumberStr = lastInvoice.InvoiceNumber.Split('-').Last();
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{prefixWithDate}{nextNumber:D4}"; // e.g., INV-2025-07-0001
        }


        public async Task<InvoiceDto?> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
        {
            // Validate patient exists
            if (!await _context.Patients.AnyAsync(p => p.Id == createInvoiceDto.PatientId))
            {
                _logger.LogWarning($"Attempted to create invoice for non-existent patient ID: {createInvoiceDto.PatientId}");
                return null;
            }

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = await GenerateNextInvoiceNumberAsync(),
                PatientId = createInvoiceDto.PatientId,
                InvoiceDate = createInvoiceDto.InvoiceDate,
                DueDate = createInvoiceDto.DueDate,
                SourceAppointmentId = createInvoiceDto.SourceAppointmentId,
                Status = InvoiceStatus.Draft, // Start as Draft
                AmountPaid = 0,
                InvoiceItems = createInvoiceDto.InvoiceItems.Select(itemDto => new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                    // TotalPrice is calculated property
                }).ToList()
            };

            // Calculate total amount from line items
            invoice.TotalAmount = invoice.InvoiceItems.Sum(item => item.TotalPrice);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Invoice {invoice.InvoiceNumber} created successfully for patient {invoice.PatientId}.");
                return await GetInvoiceByIdAsync(invoice.Id); // Re-fetch to get populated DTO
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error creating invoice for patient {createInvoiceDto.PatientId}.");
                return null;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.Patient)
                                        .Include(i => i.InvoiceItems)
                                        .FirstOrDefaultAsync(i => i.Id == invoiceId);

            return invoice == null ? null : MapInvoiceToDto(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
             var invoices = await _context.Invoices
                                        .Include(i => i.Patient)
                                        .OrderByDescending(i => i.InvoiceDate)
                                        .ToListAsync();
            return invoices.Select(MapInvoiceToDto);
        }


        public async Task<IEnumerable<InvoiceDto>> GetInvoicesForPatientAsync(Guid patientId)
        {
            var invoices = await _context.Invoices
                                        .Where(i => i.PatientId == patientId)
                                        .Include(i => i.Patient)
                                        .Include(i => i.InvoiceItems)
                                        .OrderByDescending(i => i.InvoiceDate)
                                        .ToListAsync();
            return invoices.Select(MapInvoiceToDto);
        }

        public async Task<bool> UpdateInvoiceStatusAsync(Guid invoiceId, InvoiceStatus newStatus)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                _logger.LogWarning($"Invoice with ID {invoiceId} not found for status update.");
                return false;
            }

            invoice.Status = newStatus;
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Invoice {invoice.InvoiceNumber} status updated to {newStatus}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for invoice {invoice.InvoiceNumber}.");
                return false;
            }
        }

        public async Task<InvoiceDto?> AddPaymentToInvoiceAsync(Guid invoiceId, decimal paymentAmount, DateTime paymentDate, PaymentMethod paymentMethod, string transactionId = null, string notes = null)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                _logger.LogWarning($"Invoice with ID {invoiceId} not found for payment.");
                return null;
            }

            if (paymentAmount <= 0)
            {
                _logger.LogWarning($"Invalid payment amount {paymentAmount} for invoice {invoice.InvoiceNumber}.");
                return null;
            }

            var paymentDto = new PaymentDto
            {
                InvoiceId = invoiceId,
                Amount = paymentAmount,
                PaymentDate = paymentDate,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                Notes = notes
            };

            var recordedPayment = await _paymentService.RecordPaymentAsync(paymentDto);
            if (recordedPayment == null)
            {
                _logger.LogError($"Failed to record payment for invoice {invoice.InvoiceNumber}.");
                return null;
            }

            invoice.AmountPaid += paymentAmount;
            if (invoice.AmountPaid >= invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Paid;
            }
            else
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Payment of {paymentAmount} added to invoice {invoice.InvoiceNumber}. New status: {invoice.Status}.");
                return await GetInvoiceByIdAsync(invoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding payment to invoice {invoice.InvoiceNumber}.");
                return null;
            }
        }


        private static InvoiceDto MapInvoiceToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                PatientId = invoice.PatientId,
                PatientName = invoice.Patient != null ? $"{invoice.Patient.FirstName} {invoice.Patient.LastName}".Trim() : "N/A",
                PatientCode = invoice.Patient?.PatientCode ?? "N/A",
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                Status = invoice.Status,
                InvoiceItems = invoice.InvoiceItems?.Select(item => new InvoiceItemDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList() ?? new List<InvoiceItemDto>()
            };
        }
    }
}
