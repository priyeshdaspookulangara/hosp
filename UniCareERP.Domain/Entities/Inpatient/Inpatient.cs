using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Inpatient
{
    public class Inpatient
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public string? AdmissionReason { get; set; }
        public string? DischargeReason { get; set; }
        public Inpatient()
        {
            Id = Guid.NewGuid();
            AdmissionDate = DateTime.UtcNow;
        }
    }
}
