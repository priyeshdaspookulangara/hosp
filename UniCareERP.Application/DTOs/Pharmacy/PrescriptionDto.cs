using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Pharmacy
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public ICollection<PrescriptionItemDto> PrescriptionItems { get; set; }
    }
}
