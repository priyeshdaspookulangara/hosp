using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Lab
{
    public class LabOrder
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
        public Guid LabTestId { get; set; }
        public virtual LabTest? LabTest { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty; // e.g., Ordered, Sample Collected, In Progress, Resulted
        public string? Result { get; set; }

        public LabOrder()
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.UtcNow;
        }
    }
}
