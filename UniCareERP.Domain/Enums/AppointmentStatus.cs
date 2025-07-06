namespace UniCareERP.Domain.Enums
{
    public enum AppointmentStatus
    {
        Scheduled = 1,
        Confirmed = 2,
        CancelledByPatient = 3,
        CancelledByClinic = 4,
        Completed = 5,
        NoShow = 6,
        Rescheduled = 7 // Added Rescheduled as it's a common status
    }
}
