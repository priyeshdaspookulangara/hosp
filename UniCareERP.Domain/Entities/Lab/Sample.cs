using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.Lab
{
    public class Sample
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TestRequestId { get; set; }
        public TestRequest? TestRequest { get; set; }

        [Required]
        [StringLength(100)]
        public string? SampleType { get; set; } // e.g., Blood, Urine, Saliva

        public DateTime CollectionDate { get; set; }

        [StringLength(100)]
        public string? CollectorId { get; set; } // The user who collected the sample
    }
}
