using System;
using UniCareERP.Domain.Entities; // For ApplicationUser link
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.HR
{
    public class LeaveRequest
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; }

        public DateTime RequestedDate { get; set; }

        public string? ApprovedByUserId { get; set; } // Changed to string for ApplicationUser.Id
        public virtual ApplicationUser? ApprovedByUser { get; set; }

        public DateTime? ActionDate { get; set; } // Renamed from ApprovedDate to be more generic
        public string? ApproverComments { get; set; }

        public LeaveRequest()
        {
            Id = Guid.NewGuid();
            RequestedDate = DateTime.UtcNow;
            Status = LeaveRequestStatus.Pending;
        }
    }
}
