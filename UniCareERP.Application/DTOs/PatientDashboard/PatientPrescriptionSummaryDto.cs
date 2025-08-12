using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.PatientDashboard
{
    public class PatientPrescriptionSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public PrescriptionStatus Status { get; set; }
        public int NumberOfItems { get; set; }
    }
}
