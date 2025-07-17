using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.Lab
{
    public class LabTest
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string? Category { get; set; } // e.g., Hematology, Chemistry, Microbiology

        [Required]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
    }
}
