using System;
using System.Collections.Generic;

namespace UniCareERP.Domain.Entities.Patients
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string PatientCode { get; set; } = string.Empty; // e.g., P00001
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // Consider an Enum later
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation properties (examples)
        // public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        // public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public Patient()
        {
            Id = Guid.NewGuid();
            RegistrationDate = DateTime.UtcNow;
        }
    }
}
