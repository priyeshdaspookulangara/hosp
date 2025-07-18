using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class PatientBillingService : IPatientBillingService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<PatientBillingService> _logger;

        public PatientBillingService(UniCareDbContext context, ILogger<PatientBillingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PatientAccountSummaryDto> GetPatientAccountSummaryAsync(Guid patientId)
        {
            var patientAccount = await _context.PatientAccounts
                .FirstOrDefaultAsync(pa => pa.PatientId == patientId);

            if (patientAccount == null)
            {
                // Optionally create an account if it doesn't exist
                patientAccount = new PatientAccount { PatientId = patientId };
            }

            var patient = await _context.Patients.FindAsync(patientId);

            return new PatientAccountSummaryDto
            {
                PatientId = patientId,
                PatientName = patient?.FirstName + " " + patient?.LastName,
                TotalCharges = patientAccount.TotalCharges,
                TotalPayments = patientAccount.TotalPayments,
                CurrentBalance = patientAccount.CurrentBalance
            };
        }

        public async Task<Guid> ProcessPaymentAsync(PatientPaymentDto paymentDto)
        {
            var payment = new PatientPayment
            {
                PatientId = paymentDto.PatientId,
                PaymentDate = paymentDto.PaymentDate,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod
            };

            _context.PatientPayments.Add(payment);

            var patientAccount = await _context.PatientAccounts
                .FirstOrDefaultAsync(pa => pa.PatientId == paymentDto.PatientId);

            if (patientAccount == null)
            {
                patientAccount = new PatientAccount { PatientId = paymentDto.PatientId };
                _context.PatientAccounts.Add(patientAccount);
            }

            patientAccount.TotalPayments += payment.Amount;
            patientAccount.CurrentBalance -= payment.Amount;

            await _context.SaveChangesAsync();
            return payment.Id;
        }

        public async Task<Guid> ProcessRefundAsync(PatientRefundDto refundDto)
        {
            var refund = new PatientRefund
            {
                PatientId = refundDto.PatientId,
                RefundDate = refundDto.RefundDate,
                Amount = refundDto.Amount,
                Reason = refundDto.Reason
            };

            _context.PatientRefunds.Add(refund);

            var patientAccount = await _context.PatientAccounts
                .FirstOrDefaultAsync(pa => pa.PatientId == refundDto.PatientId);

            if (patientAccount == null)
            {
                patientAccount = new PatientAccount { PatientId = refundDto.PatientId };
                _context.PatientAccounts.Add(patientAccount);
            }

            patientAccount.TotalRefunds += refund.Amount;
            patientAccount.CurrentBalance += refund.Amount;

            await _context.SaveChangesAsync();
            return refund.Id;
        }

        public async Task<PatientStatementDto> GeneratePatientStatementAsync(Guid patientId, DateTime startDate, DateTime endDate)
        {
            var statement = new PatientStatementDto
            {
                PatientId = patientId,
                StartDate = startDate,
                EndDate = endDate,
                StatementDate = DateTime.UtcNow,
                Items = new System.Collections.Generic.List<StatementItemDto>()
            };

            var invoices = await _context.Invoices
                .Where(i => i.PatientId == patientId && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .ToListAsync();

            var payments = await _context.PatientPayments
                .Where(p => p.PatientId == patientId && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToListAsync();

            var refunds = await _context.PatientRefunds
                .Where(r => r.PatientId == patientId && r.RefundDate >= startDate && r.RefundDate <= endDate)
                .ToListAsync();

            decimal balance = 0;

            foreach (var invoice in invoices)
            {
                statement.Items.Add(new StatementItemDto
                {
                    Date = invoice.InvoiceDate,
                    Description = $"Invoice #{invoice.InvoiceNumber}",
                    Charge = invoice.TotalAmount,
                    Credit = 0,
                    Balance = balance += invoice.TotalAmount
                });
            }

            foreach (var payment in payments)
            {
                statement.Items.Add(new StatementItemDto
                {
                    Date = payment.PaymentDate,
                    Description = "Payment",
                    Charge = 0,
                    Credit = payment.Amount,
                    Balance = balance -= payment.Amount
                });
            }

            foreach (var refund in refunds)
            {
                statement.Items.Add(new StatementItemDto
                {
                    Date = refund.RefundDate,
                    Description = "Refund",
                    Charge = refund.Amount,
                    Credit = 0,
                    Balance = balance += refund.Amount
                });
            }

            statement.Items = statement.Items.OrderBy(i => i.Date).ToList();

            return statement;
        }
    }
}
