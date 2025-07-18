using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities.HR
{
    public class SalaryStructure
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public decimal BasicSalary { get; set; }

        [Required]
        public decimal HouseRentAllowance { get; set; }

        [Required]
        public decimal ConveyanceAllowance { get; set; }

        [Required]
        public decimal MedicalAllowance { get; set; }

        [Required]
        public decimal ProvidentFund { get; set; }

        [Required]
        public decimal ProfessionalTax { get; set; }
    }
}
