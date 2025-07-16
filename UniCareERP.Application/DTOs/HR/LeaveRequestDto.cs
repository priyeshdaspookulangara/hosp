using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.HR
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string LeaveTypeText => LeaveType.ToString();
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public DateTime RequestedDate { get; set; }
        public string? ApproverName { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? ApproverComments { get; set; }
    }

    public class CreateLeaveRequestDto
    {
        [Required]
        public Guid EmployeeId { get; set; } // This will be set from the logged-in user context

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        // Custom validation to ensure EndDate is after StartDate
        public DateTime EndDate { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    // No UpdateLeaveRequestDto for now, as edits are handled by specific actions like Approve/Reject/Cancel
}
