using System;

namespace UniCareERP.Application.DTOs.Lab
{
    public class TestResultDto
    {
        public Guid Id { get; set; }
        public Guid TestRequestId { get; set; }
        public Guid LabTestId { get; set; }
        public string LabTestName { get; set; }
        public DateTime ResultDate { get; set; }
        public string ResultValue { get; set; }
        public string Units { get; set; }
        public string ReferenceRange { get; set; }
        public string Notes { get; set; }
    }
}
