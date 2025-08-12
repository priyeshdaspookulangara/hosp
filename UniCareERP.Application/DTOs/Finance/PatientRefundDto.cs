using System;

namespace UniCareERP.Application.DTOs.Finance
{
    public class PatientRefundDto
    {
        public Guid PatientId { get; set; }
        public DateTime RefundDate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
