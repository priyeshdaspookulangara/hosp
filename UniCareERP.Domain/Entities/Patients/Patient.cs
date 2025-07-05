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
        public string? MiddleName { get; set; }
        public string Gender { get; set; } = string.Empty; // Consider an Enum later
        public string? MaritalStatus { get; set; } // e.g., Single, Married, Divorced, Widowed (Enum later)
        public string? Nationality { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; } // Could be a complex type later

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }

        public string? BloodGroup { get; set; } // e.g., A+, O- (Enum later)
        public string? Allergies { get; set; } // Text field for known allergies

        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }

        public string? GeneralNotes { get; set; } // General registration notes

        public DateTime RegistrationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        // Navigation properties (examples)
        // public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        // public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public Patient()
        {
            Id = Guid.NewGuid();
            RegistrationDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
