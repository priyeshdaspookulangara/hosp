using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.HR
{
    public class PayrollDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime PayPeriodStartDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public List<PayslipDto> Payslips { get; set; } = new List<PayslipDto>();
    }
}
