using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance; // Assuming DTOs will be created later

namespace UniCareERP.Application.Services.Finance
{
    public interface IFinanceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync(); // Replace with InvoiceDto later
        Task<Invoice?> GetInvoiceByIdAsync(Guid id);       // Replace with InvoiceDto later
        // Add other method signatures (e.g., for GL Accounts, Payments)
    }
}
