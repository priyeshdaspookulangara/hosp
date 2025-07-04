using System;

namespace UniCareERP.Domain.Entities.Patients
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; } // Navigation to Patient
        public Guid DoctorId { get; set; } // Link to a Doctor entity (to be created, or use ApplicationUser)
        // public virtual ApplicationUser Doctor { get; set; } // If doctors are ApplicationUsers

        public DateTime AppointmentDateTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // e.g., Scheduled, Confirmed, Cancelled, Completed (Consider Enum)
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public Appointment()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            Status = "Scheduled"; // Default status
        }
    }
}
