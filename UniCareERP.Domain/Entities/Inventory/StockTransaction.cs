using System;

namespace UniCareERP.Domain.Entities.Inventory
{
    public class StockTransaction
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public virtual InventoryItem? InventoryItem { get; set; }

        public string TransactionType { get; set; } = string.Empty; // e.g., Purchase, Sale, Adjustment, Return (Enum later)
        public int QuantityChanged { get; set; } // Positive for additions, negative for deductions
        public DateTime TransactionDate { get; set; }
        public string? ReferenceId { get; set; } // e.g., PurchaseOrderId, SaleInvoiceId
        public string? Notes { get; set; }
        public Guid? UserId { get; set; } // User performing the transaction (ApplicationUser Id)

        public StockTransaction()
        {
            Id = Guid.NewGuid();
            TransactionDate = DateTime.UtcNow;
        }
    }
}
