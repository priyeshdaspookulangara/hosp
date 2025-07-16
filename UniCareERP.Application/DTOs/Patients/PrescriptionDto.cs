using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Patients
{
    public class PrescriptionItemDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class PrescriptionDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime PrescriptionDate { get; set; }
        public PrescriptionStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public List<PrescriptionItemDto> Items { get; set; } = new List<PrescriptionItemDto>();
    }

    public class CreatePrescriptionItemDto
    {
        [Required]
        public Guid InventoryItemId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class CreatePrescriptionDto
    {
        [Required]
        public Guid PatientId { get; set; }

        // DoctorId will be taken from the logged-in user context

        public Guid? SourceAppointmentId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A prescription must have at least one item.")]
        public List<CreatePrescriptionItemDto> Items { get; set; } = new List<CreatePrescriptionItemDto>();
    }
}
