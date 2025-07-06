using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Appointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty; // To be populated (e.g., Patient.FullName)
        public string PatientCode { get; set; } = string.Empty;

        public string? DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty; // To be populated (e.g., Doctor.FullName or UserName)

        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime EndDateTime => AppointmentDateTime.AddMinutes(DurationMinutes);

        public string? ServiceType { get; set; }
        public string? Reason { get; set; }
        public AppointmentStatus Status { get; set; }
        public string StatusText => Status.ToString(); // For display
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
