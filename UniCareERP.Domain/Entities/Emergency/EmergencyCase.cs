using System;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Emergency
{
    public class EmergencyCase
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public string CaseDescription { get; set; }
        public DateTime ReportedAt { get; set; }
        public EmergencyCaseStatus Status { get; set; }

        public EmergencyCase()
        {
            Id = Guid.NewGuid();
            ReportedAt = DateTime.UtcNow;
            Status = EmergencyCaseStatus.Reported;
        }
    }
}
