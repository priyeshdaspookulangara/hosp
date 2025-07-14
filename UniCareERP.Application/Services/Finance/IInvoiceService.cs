using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.Services.Finance
{
    public interface IInvoiceService
    {
        Task<InvoiceDto?> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);
        Task<InvoiceDto?> GetInvoiceByIdAsync(Guid invoiceId);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<IEnumerable<InvoiceDto>> GetInvoicesForPatientAsync(Guid patientId);
        Task<bool> UpdateInvoiceStatusAsync(Guid invoiceId, InvoiceStatus newStatus);
        Task<InvoiceDto?> AddPaymentToInvoiceAsync(Guid invoiceId, decimal paymentAmount, DateTime paymentDate); // Added paymentDate
    }
}
