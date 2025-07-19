using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Reporting
{
    public class FinanceReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal ProcedureRevenue { get; set; }
        public decimal AppointmentRevenue { get; set; }
        public decimal PharmacyRevenue { get; set; }
        public List<DoctorWiseRevenueDto> DoctorWiseRevenue { get; set; } = new List<DoctorWiseRevenueDto>();
    }

    public class DoctorWiseRevenueDto
    {
        public string DoctorName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
