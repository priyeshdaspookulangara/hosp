using System;
using System.Collections.Generic;
using UniCareERP.Application.DTOs.Billing;

namespace UniCareERP.Application.DTOs.Procedures
{
    public class ProcedureDto
    {
        public Guid Id { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public Guid? SurgeonId { get; set; }
        public DateTime ProcedureDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<ProcedureChargeDto> ProcedureCharges { get; set; } = new List<ProcedureChargeDto>();
    }
}
