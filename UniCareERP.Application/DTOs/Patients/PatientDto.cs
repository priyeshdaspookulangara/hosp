using System;

namespace UniCareERP.Application.DTOs.Patients
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {MiddleName}{(string.IsNullOrEmpty(MiddleName) ? "" : " ")}{LastName}".Trim();
        public DateTime DateOfBirth { get; set; }
        public int Age // Calculated property
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string Gender { get; set; } = string.Empty;
        public string? MaritalStatus { get; set; }
        public string? Nationality { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? BloodGroup { get; set; }
        public string? Allergies { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? GeneralNotes { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
