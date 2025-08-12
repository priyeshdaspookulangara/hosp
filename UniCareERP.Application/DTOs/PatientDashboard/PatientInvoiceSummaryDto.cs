using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.PatientDashboard
{
    public class PatientInvoiceSummaryDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
