using System;
using UniCareERP.Domain.Entities; // For ApplicationUser link if used

namespace UniCareERP.Domain.Entities.HR
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty; // e.g., EMP001

        // Link to ApplicationUser for login credentials etc. This assumes one-to-one or one-to-zero.
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // Enum later
        public DateTime HireDate { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; } // Enum or separate entity later
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; } // Personal Email, ApplicationUser.Email is for system
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true; // Employment Status

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
        public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
        public virtual SalaryStructure SalaryStructure { get; set; } = null!;

        public Employee()
        {
            Id = Guid.NewGuid();
        }
    }
}
