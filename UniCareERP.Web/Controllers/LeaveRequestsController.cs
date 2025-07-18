using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Application.Services.HR;
using UniCareERP.Domain.Entities; // For ApplicationUser
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Infrastructure.Data; // For UniCareDbContext to find Employee from UserId

namespace UniCareERP.Web.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniCareDbContext _context; // Used to find Employee from ApplicationUser
        private readonly ILogger<LeaveRequestsController> _logger;

        public LeaveRequestsController(
            ILeaveRequestService leaveRequestService,
            UserManager<ApplicationUser> userManager,
            UniCareDbContext context,
            ILogger<LeaveRequestsController> logger)
        {
            _leaveRequestService = leaveRequestService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: LeaveRequests (for the current employee)
        public async Task<IActionResult> Index()
        {
            var employee = await GetCurrentEmployeeAsync();
            if (employee == null)
            {
                // User might be an admin without an employee record
                TempData["ErrorMessage"] = "You do not have an associated employee record.";
                return View(new List<LeaveRequestDto>());
            }
            var requests = await _leaveRequestService.GetLeaveRequestsForEmployeeAsync(employee.Id);
            return View(requests);
        }

        // GET: LeaveRequests/Manage (for HR/Admins)
        [Authorize(Roles = "Admin,HRManager")]
        public async Task<IActionResult> Manage()
        {
            var pendingRequests = await _leaveRequestService.GetAllPendingLeaveRequestsAsync();
            return View(pendingRequests);
        }

        // GET: LeaveRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveRequestDto createDto)
        {
            var employee = await GetCurrentEmployeeAsync();
            if (employee == null)
            {
                ModelState.AddModelError("", "Could not find an associated employee record for your user account.");
                return View(createDto);
            }

            createDto.EmployeeId = employee.Id;

            if (ModelState.IsValid)
            {
                var result = await _leaveRequestService.CreateLeaveRequestAsync(createDto);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Leave request submitted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to submit leave request. Please check the dates and try again.");
                }
            }
            return View(createDto);
        }

        // POST: LeaveRequests/Approve/5
        [HttpPost]
        [Authorize(Roles = "Admin,HRManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var approverId = _userManager.GetUserId(User) ?? throw new UnauthorizedAccessException();
            var success = await _leaveRequestService.ApproveLeaveRequestAsync(id, approverId, "Approved via system.");
            if (success)
            {
                TempData["SuccessMessage"] = "Leave request approved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve leave request.";
            }
            return RedirectToAction(nameof(Manage));
        }

        // POST: LeaveRequests/Reject/5
        [HttpPost]
        [Authorize(Roles = "Admin,HRManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason) // Reason could come from a modal form
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                 TempData["ErrorMessage"] = "A reason is required to reject a leave request.";
                 return RedirectToAction(nameof(Manage));
            }
            var approverId = _userManager.GetUserId(User) ?? throw new UnauthorizedAccessException();
            var success = await _leaveRequestService.RejectLeaveRequestAsync(id, approverId, reason);
             if (success)
            {
                TempData["SuccessMessage"] = "Leave request rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject leave request.";
            }
            return RedirectToAction(nameof(Manage));
        }

        // POST: LeaveRequests/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var employee = await GetCurrentEmployeeAsync();
            if (employee == null)
            {
                 TempData["ErrorMessage"] = "Could not find an associated employee record.";
                 return RedirectToAction(nameof(Index));
            }
            var success = await _leaveRequestService.CancelLeaveRequestAsync(id, employee.Id);
             if (success)
            {
                TempData["SuccessMessage"] = "Leave request cancelled.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel leave request. It may have already been processed.";
            }
            return RedirectToAction(nameof(Index));
        }


        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return null;
            // This requires a round trip, but it's a reliable way to get the Employee from the ApplicationUser
            return await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }
    }
}
