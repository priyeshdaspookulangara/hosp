using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Finance
{
    public class PatientPayment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public Guid? InvoiceId { get; set; }
    }
}
