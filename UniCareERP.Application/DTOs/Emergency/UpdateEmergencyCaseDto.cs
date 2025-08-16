using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Emergency
{
    public class UpdateEmergencyCaseDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string CaseDescription { get; set; }

        [Required]
        public EmergencyCaseStatus Status { get; set; }
    }
}
