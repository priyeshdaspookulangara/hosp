using System;

namespace UniCareERP.Application.DTOs.Radiology
{
    public class RadiologyReportDto
    {
        public Guid Id { get; set; }
        public Guid RadiologyOrderId { get; set; }
        public string ReportText { get; set; }
        public Guid RadiologistId { get; set; }
        public string RadiologistName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
