using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.Services.Finance;

namespace UniCareERP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientBillingController : ControllerBase
    {
        private readonly IPatientBillingService _patientBillingService;

        public PatientBillingController(IPatientBillingService patientBillingService)
        {
            _patientBillingService = patientBillingService;
        }

        [HttpGet("account-summary/{patientId}")]
        public async Task<IActionResult> GetAccountSummary(Guid patientId)
        {
            var summary = await _patientBillingService.GetPatientAccountSummaryAsync(patientId);
            return Ok(summary);
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PatientPaymentDto paymentDto)
        {
            var paymentId = await _patientBillingService.ProcessPaymentAsync(paymentDto);
            return Ok(new { PaymentId = paymentId });
        }

        [HttpPost("process-refund")]
        public async Task<IActionResult> ProcessRefund([FromBody] PatientRefundDto refundDto)
        {
            var refundId = await _patientBillingService.ProcessRefundAsync(refundDto);
            return Ok(new { RefundId = refundId });
        }

        [HttpGet("patient-statement")]
        public async Task<IActionResult> GenerateStatement(Guid patientId, DateTime startDate, DateTime endDate)
        {
            var statement = await _patientBillingService.GeneratePatientStatementAsync(patientId, startDate, endDate);
            return Ok(statement);
        }
    }
}
