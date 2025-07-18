using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.HR
{
    public class Payslip
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PayrollId { get; set; }
        public Payroll Payroll { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
