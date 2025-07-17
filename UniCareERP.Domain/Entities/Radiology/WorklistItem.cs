using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class WorklistItem
    {
        public Guid Id { get; set; }
        public Guid ImagingTestId { get; set; }
        public ImagingTest ImagingTest { get; set; }
        public string Status { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
