using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Patients
{
    public class UpdatePatientDto
    {
        [Required]
        public Guid Id { get; set; }

        // PatientCode is usually not updatable directly by user after creation
        // public string PatientCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(50)]
        public string? PreferredLanguage { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Allergies { get; set; }

        [MaxLength(100)]
        public string? InsuranceProvider { get; set; }

        [MaxLength(50)]
        public string? InsurancePolicyNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string? GeneralNotes { get; set; }
    }
}
