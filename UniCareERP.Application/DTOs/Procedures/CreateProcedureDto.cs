using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Procedures
{
    public class CreateProcedureDto
    {
        [Required]
        public string ProcedureName { get; set; } = string.Empty;

        [Required]
        public Guid PatientId { get; set; }

        public Guid? SurgeonId { get; set; }

        [Required]
        public DateTime ProcedureDate { get; set; }

        public string? Notes { get; set; }
    }
}
