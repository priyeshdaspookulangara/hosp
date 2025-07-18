using System;

namespace UniCareERP.Application.DTOs.Billing
{
    public class ProcedureChargeDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ProcedureId { get; set; }
        public string ChargeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
