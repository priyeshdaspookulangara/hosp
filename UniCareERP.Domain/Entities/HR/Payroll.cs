using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.HR
{
    public class Payroll
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [Required]
        public DateTime PayPeriodStartDate { get; set; }

        [Required]
        public DateTime PayPeriodEndDate { get; set; }

        [Required]
        public decimal GrossSalary { get; set; }

        [Required]
        public decimal NetSalary { get; set; }

        [Required]
        public decimal TotalDeductions { get; set; }

        public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}
