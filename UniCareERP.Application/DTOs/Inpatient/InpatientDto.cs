using System;

namespace UniCareERP.Application.DTOs.Inpatient
{
    public class InpatientDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string RoomNumber { get; set; }
        public string BedNumber { get; set; }
        public string AdmissionReason { get; set; }
        public string DischargeReason { get; set; }
    }
}
