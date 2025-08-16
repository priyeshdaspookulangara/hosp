using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Emergency
{
    public class CreateEmergencyCaseDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [StringLength(500)]
        public string CaseDescription { get; set; }
    }
}
