using System;

namespace UniCareERP.Application.DTOs.Radiology
{
    public class RadiologyOrderDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid TestTypeId { get; set; }
        public string TestTypeName { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; }
    }
}
