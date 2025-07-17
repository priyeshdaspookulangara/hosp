using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Finance
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
    }
}
