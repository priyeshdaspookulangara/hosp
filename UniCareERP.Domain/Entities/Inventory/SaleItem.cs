using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class SaleItem
    {
        public Guid Id { get; set; }

        public Guid SaleId { get; set; }
        public virtual Sale? Sale { get; set; }

        public Guid InventoryItemId { get; set; }
        public virtual InventoryItem? InventoryItem { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; } // Selling price at the time of sale

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice => Quantity * UnitPrice;

        public SaleItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
