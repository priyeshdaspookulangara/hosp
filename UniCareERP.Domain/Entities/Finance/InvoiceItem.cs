using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Finance
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public string Description { get; set; } = string.Empty; // e.g., "Consultation Fee", "Lab Test: CBC"

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice => Quantity * UnitPrice;

        // Optional: Could link to a Product/Service entity
        public Guid? ServiceId { get; set; }

        public InvoiceItem()
        {
            Id = Guid.NewGuid();
            Quantity = 1;
        }
    }
}
