using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor selection is required.")]
        public string? DoctorId { get; set; } // ApplicationUser.Id

        [Required]
        [DataType(DataType.DateTime)]
        // Add custom validation for future date, business hours if needed
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        [Range(5, 240, ErrorMessage = "Duration must be between 5 and 240 minutes.")] // Example range
        public int DurationMinutes { get; set; } = 30; // Default duration

        [MaxLength(100)]
        public string? ServiceType { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        // Status will be set by the service, typically to 'Scheduled' initially.
        // public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
