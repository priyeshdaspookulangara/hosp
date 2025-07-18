using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.PatientDashboard
{
    public class PatientAppointmentSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string DoctorName { get; set; }
        public string ServiceType { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
