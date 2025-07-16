namespace UniCareERP.Domain.Enums
{
    public enum PrescriptionStatus
    {
        Active = 1,     // New prescription, available for dispensing
        Dispensed = 2,  // Fully dispensed by the pharmacy
        Cancelled = 3,  // Cancelled by the doctor
        Expired = 4     // Prescription is past its valid date
    }
}
