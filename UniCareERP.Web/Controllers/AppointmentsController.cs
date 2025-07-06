using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Application.Services.Appointments;
using UniCareERP.Application.Services.Patients; // To get patient list
using UniCareERP.Domain.Entities; // For ApplicationUser
using UniCareERP.Domain.Enums; // For AppointmentStatus

namespace UniCareERP.Web.Controllers
{
    [Authorize] // Requires login, can add roles later e.g. [Authorize(Roles = "Admin,Receptionist,Doctor")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService; // To fetch patients for dropdown
        private readonly UserManager<ApplicationUser> _userManager; // To fetch doctors for dropdown
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            UserManager<ApplicationUser> userManager,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(DateTime? searchDate, string? doctorId)
        {
            ViewData["CurrentSearchDate"] = searchDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["CurrentDoctorId"] = doctorId;

            await PopulateDoctors ViewBag();

            var appointments = Enumerable.Empty<AppointmentDto>();
            DateTime dateToQuery = searchDate ?? DateTime.Today;

            if (!string.IsNullOrEmpty(doctorId))
            {
                appointments = await _appointmentService.GetAppointmentsForDoctorAsync(doctorId, dateToQuery, dateToQuery);
            }
            else
            {
                appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(dateToQuery, dateToQuery);
            }

            return View(appointments.OrderBy(a => a.AppointmentDateTime));
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id.Value);
            if (appointment == null) return NotFound();
            return View(appointment);
        }


        // GET: Appointments/Create
        public async Task<IActionResult> Create(Guid? patientId, DateTime? initialDate)
        {
            await PopulatePatientsAndDoctorsViewBag();
            var model = new CreateAppointmentDto
            {
                PatientId = patientId ?? Guid.Empty,
                AppointmentDateTime = initialDate ?? DateTime.Now.Date.AddHours(9), // Default to today 9 AM
                DurationMinutes = 30
            };
            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentDto createAppointmentDto)
        {
            if (ModelState.IsValid)
            {
                var scheduledAppointment = await _appointmentService.ScheduleAppointmentAsync(createAppointmentDto);
                if (scheduledAppointment != null)
                {
                    TempData["SuccessMessage"] = $"Appointment for {scheduledAppointment.PatientName} with {scheduledAppointment.DoctorName} on {scheduledAppointment.AppointmentDateTime:g} scheduled successfully.";
                    return RedirectToAction(nameof(Index), new { searchDate = scheduledAppointment.AppointmentDateTime.Date, doctorId = scheduledAppointment.DoctorId });
                }
                else
                {
                    // Service layer should ideally give more specific error, or we add one here based on common issues
                    ModelState.AddModelError(string.Empty, "Failed to schedule appointment. Please check patient, doctor, and ensure no time conflicts.");
                }
            }
            await PopulatePatientsAndDoctorsViewBag(createAppointmentDto.PatientId, createAppointmentDto.DoctorId);
            return View(createAppointmentDto);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id.Value);
            if (appointmentDto == null) return NotFound();

            var updateDto = new UpdateAppointmentDto
            {
                Id = appointmentDto.Id,
                PatientId = appointmentDto.PatientId,
                DoctorId = appointmentDto.DoctorId,
                AppointmentDateTime = appointmentDto.AppointmentDateTime,
                DurationMinutes = appointmentDto.DurationMinutes,
                ServiceType = appointmentDto.ServiceType,
                Reason = appointmentDto.Reason,
                Status = appointmentDto.Status,
                Notes = appointmentDto.Notes
            };
            await PopulatePatientsAndDoctorsViewBag(updateDto.PatientId, updateDto.DoctorId);
            PopulateAppointmentStatusViewBag(updateDto.Status);

            ViewBag.PatientName = appointmentDto.PatientName; // For display in Edit view
            ViewBag.PatientCode = appointmentDto.PatientCode; // For display in Edit view

            return View(updateDto);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateAppointmentDto updateAppointmentDto)
        {
            if (id != updateAppointmentDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(updateAppointmentDto);
                if (updatedAppointment != null)
                {
                    TempData["SuccessMessage"] = "Appointment updated successfully.";
                    return RedirectToAction(nameof(Index), new { searchDate = updatedAppointment.AppointmentDateTime.Date, doctorId = updatedAppointment.DoctorId });
                }
                else
                {
                     ModelState.AddModelError(string.Empty, "Failed to update appointment. Please check for conflicts or other issues.");
                }
            }
            await PopulatePatientsAndDoctorsViewBag(updateAppointmentDto.PatientId, updateAppointmentDto.DoctorId);
            PopulateAppointmentStatusViewBag(updateAppointmentDto.Status);
            return View(updateAppointmentDto);
        }

        // POST: Appointments/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, string cancellationReason = "Cancelled by staff") // Simplified reason
        {
            // In a real app, you might have a dedicated view/modal for cancellation reason
            var success = await _appointmentService.CancelAppointmentAsync(id, cancellationReason, false); // false = not cancelled by patient
            if(success)
            {
                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel appointment.";
            }
             // Redirect to Index, ideally retaining filters or redirecting to a relevant page.
             // For simplicity, redirecting to the default Index view.
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulatePatientsAndDoctorsViewBag(Guid? selectedPatientId = null, string? selectedDoctorId = null)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.PatientId = new SelectList(patients.OrderBy(p => p.FullName), "Id", "FullName", selectedPatientId);

            await PopulateDoctorsViewBag(selectedDoctorId);
        }

        private async Task PopulateDoctorsViewBag(string? selectedDoctorId = null)
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor"); // Assuming "Doctor" role exists
            ViewBag.DoctorId = new SelectList(doctors.OrderBy(u => u.UserName), "Id", "UserName", selectedDoctorId);
            // Consider displaying FullName if available: .Select(u => new { Id = u.Id, Name = $"{u.FirstName} {u.LastName}".Trim() })
        }


        private void PopulateAppointmentStatusViewBag(AppointmentStatus? selectedStatus = null)
        {
            var statuses = Enum.GetValues(typeof(AppointmentStatus))
                               .Cast<AppointmentStatus>()
                               .Select(e => new SelectListItem
                               {
                                   Value = e.ToString(),
                                   Text = e.ToString().Replace("CancelledBy", "Cancelled By "), // Add spaces for readability
                                   Selected = (e == selectedStatus)
                               });
            ViewBag.Status = new SelectList(statuses, "Value", "Text", selectedStatus?.ToString());
        }
    }
}
