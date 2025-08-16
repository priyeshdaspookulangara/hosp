using System;

namespace UniCareERP.Application.DTOs.Radiology
{
    public class RadiologyImageDto
    {
        public Guid Id { get; set; }
        public Guid RadiologyOrderId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
