using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class RadiologyReport
    {
        public Guid Id { get; set; }
        public Guid ImagingTestId { get; set; }
        public ImagingTest ImagingTest { get; set; }
        public Guid RadiologistId { get; set; }
        public ApplicationUser Radiologist { get; set; }
        public string ReportText { get; set; }
        public DateTime ReportDate { get; set; }
    }
}
