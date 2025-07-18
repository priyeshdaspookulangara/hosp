using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums; // For InvoiceStatus enum

namespace UniCareERP.Domain.Entities.Finance
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaid { get; set; }

        public InvoiceStatus Status { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<ProcedureCharge> ProcedureCharges { get; set; } = new List<ProcedureCharge>();

        // Optional: Reference to the appointment that generated this invoice
        public Guid? SourceAppointmentId { get; set; }

        public Invoice()
        {
            Id = Guid.NewGuid();
            InvoiceDate = DateTime.UtcNow;
            DueDate = DateTime.UtcNow.AddDays(30); // Default due date
            Status = InvoiceStatus.Draft;
        }
    }
}
