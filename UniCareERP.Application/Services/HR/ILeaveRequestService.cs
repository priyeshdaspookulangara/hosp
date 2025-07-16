using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;

namespace UniCareERP.Application.Services.HR
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDto?> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto);
        Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid requestId);
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsForEmployeeAsync(Guid employeeId);
        Task<IEnumerable<LeaveRequestDto>> GetAllPendingLeaveRequestsAsync();
        Task<bool> ApproveLeaveRequestAsync(Guid requestId, string approverId, string? comments);
        Task<bool> RejectLeaveRequestAsync(Guid requestId, string approverId, string comments);
        Task<bool> CancelLeaveRequestAsync(Guid requestId, Guid employeeId);
    }
}
