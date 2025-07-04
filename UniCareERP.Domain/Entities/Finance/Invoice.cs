using System;
using System.Collections.Generic;
using UniCareERP.Domain.Entities.Patients; // For Patient link

namespace UniCareERP.Domain.Entities.Finance
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty; // e.g., INV-2025-00001
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; } // Navigation to Patient

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } = string.Empty; // e.g., Draft, Sent, Paid, PartiallyPaid, Overdue (Enum later)

        // public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        // public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public Invoice()
        {
            Id = Guid.NewGuid();
            InvoiceDate = DateTime.UtcNow;
            Status = "Draft";
        }
    }

    // Example for InvoiceItem (can be expanded later)
    // public class InvoiceItem
    // {
    //     public Guid Id { get; set; }
    //     public Guid InvoiceId { get; set; }
    //     public virtual Invoice Invoice { get; set; }
    //     public string Description { get; set; } = string.Empty;
    //     public int Quantity { get; set; }
    //     public decimal UnitPrice { get; set; }
    //     public decimal TotalPrice => Quantity * UnitPrice;
    // }
}
