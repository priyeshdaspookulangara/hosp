using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.HR
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<LeaveRequestService> _logger;

        public LeaveRequestService(UniCareDbContext context, ILogger<LeaveRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LeaveRequestDto?> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto)
        {
            // Basic validation
            if (createDto.StartDate > createDto.EndDate)
            {
                _logger.LogWarning($"Leave request failed: Start date {createDto.StartDate} is after end date {createDto.EndDate}.");
                return null;
            }

            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = createDto.EmployeeId,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                LeaveType = createDto.LeaveType,
                Reason = createDto.Reason,
                Status = LeaveRequestStatus.Pending,
                RequestedDate = DateTime.UtcNow
            };

            try
            {
                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Leave request {leaveRequest.Id} created for employee {leaveRequest.EmployeeId}.");
                return await GetLeaveRequestByIdAsync(leaveRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request.");
                return null;
            }
        }

        public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid requestId)
        {
            var request = await _context.LeaveRequests
                                        .Include(lr => lr.Employee)
                                        .Include(lr => lr.ApprovedByUser)
                                        .FirstOrDefaultAsync(lr => lr.Id == requestId);

            return request == null ? null : MapLeaveRequestToDto(request);
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsForEmployeeAsync(Guid employeeId)
        {
            var requests = await _context.LeaveRequests
                                         .Where(lr => lr.EmployeeId == employeeId)
                                         .Include(lr => lr.Employee)
                                         .Include(lr => lr.ApprovedByUser)
                                         .OrderByDescending(lr => lr.RequestedDate)
                                         .ToListAsync();

            return requests.Select(MapLeaveRequestToDto);
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetAllPendingLeaveRequestsAsync()
        {
            var requests = await _context.LeaveRequests
                                         .Where(lr => lr.Status == LeaveRequestStatus.Pending)
                                         .Include(lr => lr.Employee)
                                         .OrderBy(lr => lr.RequestedDate)
                                         .ToListAsync();

            return requests.Select(MapLeaveRequestToDto);
        }

        public async Task<bool> ApproveLeaveRequestAsync(Guid requestId, string approverId, string? comments)
        {
            return await UpdateLeaveRequestStatusAsync(requestId, approverId, LeaveRequestStatus.Approved, comments);
        }

        public async Task<bool> RejectLeaveRequestAsync(Guid requestId, string approverId, string comments)
        {
            return await UpdateLeaveRequestStatusAsync(requestId, approverId, LeaveRequestStatus.Rejected, comments);
        }

        public async Task<bool> CancelLeaveRequestAsync(Guid requestId, Guid employeeId)
        {
            var request = await _context.LeaveRequests.FindAsync(requestId);
            if (request == null || request.EmployeeId != employeeId || request.Status != LeaveRequestStatus.Pending)
            {
                _logger.LogWarning($"Failed to cancel leave request {requestId}. Not found, not owned by employee {employeeId}, or not in pending state.");
                return false;
            }

            request.Status = LeaveRequestStatus.Cancelled;
            request.ActionDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Leave request {requestId} cancelled by employee.");
                return true;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, $"Error cancelling leave request {requestId}.");
                return false;
            }
        }

        private async Task<bool> UpdateLeaveRequestStatusAsync(Guid requestId, string approverId, LeaveRequestStatus newStatus, string? comments)
        {
            var request = await _context.LeaveRequests.FindAsync(requestId);
            if (request == null || request.Status != LeaveRequestStatus.Pending)
            {
                _logger.LogWarning($"Leave request {requestId} not found or not in pending state for approval/rejection.");
                return false;
            }

            request.Status = newStatus;
            request.ApprovedByUserId = approverId;
            request.ApproverComments = comments;
            request.ActionDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Leave request {requestId} status updated to {newStatus} by approver {approverId}.");
                // In a real app, you would trigger a notification to the employee here.
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for leave request {requestId}.");
                return false;
            }
        }

        private static LeaveRequestDto MapLeaveRequestToDto(LeaveRequest request)
        {
            return new LeaveRequestDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                EmployeeName = request.Employee != null ? $"{request.Employee.FirstName} {request.Employee.LastName}" : "N/A",
                EmployeeCode = request.Employee?.EmployeeCode ?? "N/A",
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                LeaveType = request.LeaveType,
                Reason = request.Reason,
                Status = request.Status,
                RequestedDate = request.RequestedDate,
                ApproverName = request.ApprovedByUser != null ? $"{request.ApprovedByUser.FirstName} {request.ApprovedByUser.LastName}".Trim() : null,
                ActionDate = request.ActionDate,
                ApproverComments = request.ApproverComments
            };
        }
    }
}
