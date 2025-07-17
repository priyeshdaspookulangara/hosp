using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IPaymentService
    {
        Task<PaymentDto> GetPaymentByIdAsync(Guid id);
        Task<IEnumerable<PaymentDto>> GetPaymentsForInvoiceAsync(Guid invoiceId);
        Task<PaymentDto> RecordPaymentAsync(PaymentDto paymentDto);
    }
}
