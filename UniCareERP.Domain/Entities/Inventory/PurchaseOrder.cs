using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty; // e.g., PO-2025-0001
        public string? SupplierInfo { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

        public PurchaseOrder()
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.UtcNow;
            Status = PurchaseOrderStatus.Pending;
        }
    }
}
