namespace UniCareERP.Domain.Enums
{
    public enum PurchaseOrderStatus
    {
        Pending = 1,        // PO created, awaiting approval
        Approved = 2,       // PO approved, awaiting delivery
        PartiallyReceived = 3, // Some items have been received
        Completed = 4,      // All items have been received
        Cancelled = 5       // PO cancelled
    }
}
