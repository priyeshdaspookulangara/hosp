using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Finance
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; } // For external payment gateway reference
        public string? Notes { get; set; }
    }
}
