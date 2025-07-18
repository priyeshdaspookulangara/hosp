using System;

namespace UniCareERP.Application.DTOs.Lab
{
    public class LabOrderDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid LabTestId { get; set; }
        public string LabTestName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Result { get; set; }
    }
}
