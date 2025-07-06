using System;
using UniCareERP.Domain.Enums; // For AppointmentStatus enum
using UniCareERP.Domain.Entities; // For ApplicationUser

namespace UniCareERP.Domain.Entities.Patients
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public string? DoctorId { get; set; } // Changed to string? to link to ApplicationUser.Id
        public virtual ApplicationUser? Doctor { get; set; } // Navigation to ApplicationUser

        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; } // e.g., 15, 30, 60 minutes
        public string? ServiceType { get; set; } // e.g., "Consultation", "Follow-up", "Procedure X"

        public string? Reason { get; set; } // Reason for the visit
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; } // General notes for the appointment

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public Appointment()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
            Status = AppointmentStatus.Scheduled; // Default status
            DurationMinutes = 30; // Default duration
        }
    }
}
