using System;

namespace UniCareERP.Domain.Entities.HR
{
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = string.Empty; // e.g., Annual, Sick, Unpaid (Enum later)
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // e.g., Pending, Approved, Rejected (Enum later)
        public DateTime RequestedDate { get; set; }
        public Guid? ApprovedByUserId { get; set; } // Link to ApplicationUser (Manager/HR)
        // public virtual ApplicationUser ApprovedByUser {get; set;}
        public DateTime? ApprovedDate { get; set; }

        public LeaveRequest()
        {
            Id = Guid.NewGuid();
            RequestedDate = DateTime.UtcNow;
            Status = "Pending";
        }
    }
}
