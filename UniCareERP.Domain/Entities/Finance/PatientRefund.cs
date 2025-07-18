using System;

namespace UniCareERP.Domain.Entities.Finance
{
    public class PatientRefund
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime RefundDate { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public Guid IssuedBy { get; set; }
    }
}
