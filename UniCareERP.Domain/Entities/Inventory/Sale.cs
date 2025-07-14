using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class Sale
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty; // e.g., SALE-2025-00001

        public DateTime SaleDate { get; set; }

        public Guid? PatientId { get; set; } // Nullable for walk-in customers
        public virtual Patient? Patient { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

        public Sale()
        {
            Id = Guid.NewGuid();
            SaleDate = DateTime.UtcNow;
        }
    }
}
