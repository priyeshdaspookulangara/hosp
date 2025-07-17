using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.Lab
{
    public class TestResult
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TestRequestId { get; set; }
        public TestRequest? TestRequest { get; set; }

        [Required]
        public Guid LabTestId { get; set; }
        public LabTest? LabTest { get; set; }

        [Required]
        public DateTime ResultDate { get; set; }

        [Required]
        public string? ResultValue { get; set; } // The result of the test

        [StringLength(100)]
        public string? Units { get; set; } // e.g., mg/dL, mmol/L

        [StringLength(500)]
        public string? ReferenceRange { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
