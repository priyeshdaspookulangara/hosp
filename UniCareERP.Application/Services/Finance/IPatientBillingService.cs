using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IPatientBillingService
    {
        Task<PatientAccountSummaryDto> GetPatientAccountSummaryAsync(Guid patientId);
        Task<Guid> ProcessPaymentAsync(PatientPaymentDto paymentDto);
        Task<Guid> ProcessRefundAsync(PatientRefundDto refundDto);
        Task<PatientStatementDto> GeneratePatientStatementAsync(Guid patientId, DateTime startDate, DateTime endDate);
    }
}
