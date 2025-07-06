using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Appointments
{
    public class UpdateAppointmentDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; } // Usually not changed, but good for validation

        [Required(ErrorMessage = "Doctor selection is required.")]
        public string? DoctorId { get; set; } // ApplicationUser.Id

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        [Range(5, 240, ErrorMessage = "Duration must be between 5 and 240 minutes.")]
        public int DurationMinutes { get; set; }

        [MaxLength(100)]
        public string? ServiceType { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
