using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IPaymentService
    {
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task ProcessPaymentAsync(Guid invoiceId, decimal amount);
    }
}
