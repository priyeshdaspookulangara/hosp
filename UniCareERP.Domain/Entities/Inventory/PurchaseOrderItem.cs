using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class PurchaseOrderItem
    {
        public Guid Id { get; set; }

        public Guid PurchaseOrderId { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        public Guid InventoryItemId { get; set; }
        public virtual InventoryItem? InventoryItem { get; set; }

        public int QuantityOrdered { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; } // The cost price at the time of order

        public int QuantityReceived { get; set; }

        public PurchaseOrderItem()
        {
            Id = Guid.NewGuid();
            QuantityReceived = 0;
        }
    }
}
