using System;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class InventoryItem // Could be Medicine, Supply, etc.
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty; // e.g., MED0001, SUP0001
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty; // Medicine, Supply, Equipment (Enum or separate entity later)
        public string UnitOfMeasure { get; set; } = string.Empty; // e.g., Pcs, Box, Bottle (Enum later)

        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; } // Selling price
        public decimal CostPrice { get; set; }   // Purchase price

        public int ReorderLevel { get; set; }
        public string? SupplierInfo { get; set; } // Or link to a Supplier entity
        public DateTime? ExpiryDate { get; set; } // Relevant for medicines
        public string? BatchNumber { get; set; }
        public bool IsActive { get; set; } = true;

        public InventoryItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
