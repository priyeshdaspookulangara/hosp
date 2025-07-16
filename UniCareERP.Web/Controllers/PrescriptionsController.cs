using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;
using UniCareERP.Application.Services.Inventory; // For item search
using UniCareERP.Application.Services.Patients;
using UniCareERP.Domain.Entities;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientService _patientService;
        private readonly IInventoryService _inventoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IPatientService patientService,
            IInventoryService inventoryService,
            UserManager<ApplicationUser> userManager)
        {
            _prescriptionService = prescriptionService;
            _patientService = patientService;
            _inventoryService = inventoryService;
            _userManager = userManager;
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id.Value);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        // GET: Prescriptions/Create
        public async Task<IActionResult> Create(Guid patientId, Guid? appointmentId)
        {
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null) return NotFound();

            ViewBag.PatientName = patient.FullName;
            ViewBag.PatientCode = patient.PatientCode;

            var model = new CreatePrescriptionDto
            {
                PatientId = patientId,
                SourceAppointmentId = appointmentId
            };
            return View(model);
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePrescriptionDto createDto)
        {
            createDto.Items.RemoveAll(item =>
                string.IsNullOrWhiteSpace(item.Dosage) &&
                string.IsNullOrWhiteSpace(item.Frequency) &&
                string.IsNullOrWhiteSpace(item.Duration));

            if (ModelState.IsValid)
            {
                var doctorId = _userManager.GetUserId(User) ?? throw new UnauthorizedAccessException();
                var createdPrescription = await _prescriptionService.CreatePrescriptionAsync(createDto, doctorId);

                if (createdPrescription != null)
                {
                    TempData["SuccessMessage"] = "Prescription created successfully.";
                    // Redirect to patient details to see the new prescription in their list
                    return RedirectToAction("Details", "Patients", new { id = createdPrescription.PatientId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create prescription. Please check that all selected items are valid drugs.");
                }
            }

            var patient = await _patientService.GetPatientByIdAsync(createDto.PatientId);
            ViewBag.PatientName = patient?.FullName;
            ViewBag.PatientCode = patient?.PatientCode;
            return View(createDto);
        }

        // POST: Prescriptions/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = _userManager.GetUserId(User) ?? throw new UnauthorizedAccessException();
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id); // Fetch to get patient id for redirect
            if(prescription == null) return NotFound();

            var success = await _prescriptionService.CancelPrescriptionAsync(id, userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Prescription cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel prescription. It may have already been dispensed or you may not be the prescribing doctor.";
            }
            return RedirectToAction("Details", "Patients", new { id = prescription.PatientId });
        }
    }
}
