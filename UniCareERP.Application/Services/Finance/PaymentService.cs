using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class PaymentService : IPaymentService
    {
        private readonly UniCareDbContext _context;

        public PaymentService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await _context.Invoices
                .Where(i => i.Status != "Paid")
                .ToListAsync();
        }

        public async Task ProcessPaymentAsync(Guid invoiceId, decimal amount)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                invoice.AmountPaid += amount;
                if (invoice.AmountPaid >= invoice.TotalAmount)
                {
                    invoice.Status = "Paid";
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
