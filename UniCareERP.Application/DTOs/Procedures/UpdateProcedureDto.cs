using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Procedures
{
    public class UpdateProcedureDto
    {
        [Required]
        public Guid Id { get; set; }

        public string? ProcedureName { get; set; }

        public Guid? SurgeonId { get; set; }

        public DateTime? ProcedureDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public ProcedureStatus? Status { get; set; }

        public string? Notes { get; set; }
    }
}
