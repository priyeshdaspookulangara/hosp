using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class PaymentService : IPaymentService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(UniCareDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.Id == id);

            return payment == null ? null : MapPaymentToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsForInvoiceAsync(Guid invoiceId)
        {
            var payments = await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .Include(p => p.Invoice)
                .ToListAsync();

            return payments.Select(MapPaymentToDto);
        }

        public async Task<PaymentDto> RecordPaymentAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = paymentDto.InvoiceId,
                PaymentDate = paymentDto.PaymentDate,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                TransactionId = paymentDto.TransactionId,
                Notes = paymentDto.Notes
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Payment {payment.Id} recorded for invoice {payment.InvoiceId}.");
                return await GetPaymentByIdAsync(payment.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error recording payment for invoice {paymentDto.InvoiceId}.");
                return null;
            }
        }

        private static PaymentDto MapPaymentToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.Invoice?.InvoiceNumber,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                Notes = payment.Notes
            };
        }
    }
}
