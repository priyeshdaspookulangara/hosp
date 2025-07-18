using System;

namespace UniCareERP.Application.DTOs.Finance
{
    public class PatientAccountSummaryDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
