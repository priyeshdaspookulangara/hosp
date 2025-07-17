using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class ImagingTest
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public Guid ImagingServiceId { get; set; }
        public ImagingService ImagingService { get; set; }
        public DateTime TestDate { get; set; }
        public string Status { get; set; }
    }
}
