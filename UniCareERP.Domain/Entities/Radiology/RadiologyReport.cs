using System;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class RadiologyReport
    {
        public Guid Id { get; set; }
        public Guid RadiologyOrderId { get; set; }
        public virtual RadiologyOrder RadiologyOrder { get; set; }
        public string ReportText { get; set; }
        public Guid RadiologistId { get; set; } // This would link to an Employee/User
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
