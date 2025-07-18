using System;
using System.Collections.Generic;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Pharmacy
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
    }
}
