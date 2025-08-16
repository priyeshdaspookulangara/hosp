using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Emergency
{
    public class EmergencyCaseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public string CaseDescription { get; set; }
        public DateTime ReportedAt { get; set; }
        public EmergencyCaseStatus Status { get; set; }
    }
}
