using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Reporting
{
    public class ProcedureReportDto
    {
        public Guid ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string SurgeonName { get; set; } = string.Empty;
        public DateTime ProcedureDate { get; set; }
        public decimal TotalCharges { get; set; }
    }
}
