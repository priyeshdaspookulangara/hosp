namespace UniCareERP.Domain.Enums
{
    public enum ChargeStatus
    {
        Pending,        // Charge has been created but not yet posted to an invoice
        PostedToInvoice, // Charge is on a patient's invoice
        Cancelled       // Charge was voided
    }
}
