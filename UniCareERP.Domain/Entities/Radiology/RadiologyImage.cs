using System;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class RadiologyImage
    {
        public Guid Id { get; set; }
        public Guid RadiologyOrderId { get; set; }
        public virtual RadiologyOrder RadiologyOrder { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
