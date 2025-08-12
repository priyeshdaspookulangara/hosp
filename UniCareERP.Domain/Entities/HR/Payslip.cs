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
        public Payroll Payroll { get; set; } = null!;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }
    }
}
