using System;
using System.Collections.Generic;
using UniCareERP.Domain.Entities; // For ApplicationUser
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Patients
{
    public class Prescription
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public string DoctorId { get; set; } = string.Empty;
        public virtual ApplicationUser? Doctor { get; set; }

        public DateTime PrescriptionDate { get; set; }

        public PrescriptionStatus Status { get; set; }

        // Optional: Reference to the appointment where this was prescribed
        public Guid? SourceAppointmentId { get; set; }
        public virtual Appointment? SourceAppointment { get; set; }

        public virtual ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();

        public Prescription()
        {
            Id = Guid.NewGuid();
            PrescriptionDate = DateTime.UtcNow;
            Status = PrescriptionStatus.Active;
        }
    }
}
