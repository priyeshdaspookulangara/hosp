using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.HR
{
    public class LeaveBalance
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        [Required]
        public decimal EntitledDays { get; set; }

        [Required]
        public decimal UsedDays { get; set; }

        public decimal RemainingDays => EntitledDays - UsedDays;
    }
}
