using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class RadiologyOrder
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public Guid TestTypeId { get; set; }
        public virtual RadiologyTest TestType { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; } // e.g., Scheduled, In Progress, Completed, Cancelled
    }
}
