namespace UniCareERP.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,      // Invoice created but not yet finalized/sent
        Sent = 2,       // Invoice sent to the patient/payer
        Paid = 3,       // Invoice paid in full
        PartiallyPaid = 4,
        Overdue = 5,    // Past due date and not fully paid
        Void = 6        // Invoice cancelled and is no longer valid
    }
}
