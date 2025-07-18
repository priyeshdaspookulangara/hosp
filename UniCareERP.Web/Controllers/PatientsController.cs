using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Application.DTOs.Patients;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.Services.Appointments;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.Patients; // Added for IPrescriptionService


namespace UniCareERP.Web.Controllers
{
    [Authorize] // Assuming all patient actions require login. Adjust roles as needed.
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPrescriptionService _prescriptionService; // Added
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(
            IPatientService patientService,
            IAppointmentService appointmentService,
            IInvoiceService invoiceService,
            IPrescriptionService prescriptionService, // Added
            ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _invoiceService = invoiceService;
            _prescriptionService = prescriptionService; // Added
            _logger = logger;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return View(patients);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }

            // Fetch appointments for this patient
            ViewBag.PatientAppointments = await _appointmentService.GetAppointmentsForPatientAsync(id.Value);

            // Fetch invoices for this patient
            ViewBag.PatientInvoices = await _invoiceService.GetInvoicesForPatientAsync(id.Value);

            // Fetch prescriptions for this patient
            ViewBag.PatientPrescriptions = await _prescriptionService.GetPrescriptionsForPatientAsync(id.Value);

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientDto createPatientDto)
        {
            if (ModelState.IsValid)
            {
                var createdPatient = await _patientService.CreatePatientAsync(createPatientDto);
                if (createdPatient != null)
                {
                    TempData["SuccessMessage"] = $"Patient {createdPatient.FullName} ({createdPatient.PatientCode}) registered successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "An error occurred while creating the patient. Please try again.");
            }
            return View(createPatientDto);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDto = await _patientService.GetPatientByIdAsync(id.Value);
            if (patientDto == null)
            {
                return NotFound();
            }

            // Map PatientDto to UpdatePatientDto (manual mapping for now)
            var updatePatientDto = new UpdatePatientDto
            {
                Id = patientDto.Id,
                FirstName = patientDto.FirstName,
                MiddleName = patientDto.MiddleName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                Gender = patientDto.Gender,
                MaritalStatus = patientDto.MaritalStatus,
                Nationality = patientDto.Nationality,
                PreferredLanguage = patientDto.PreferredLanguage,
                PhoneNumber = patientDto.PhoneNumber,
                Email = patientDto.Email,
                Address = patientDto.Address,
                EmergencyContactName = patientDto.EmergencyContactName,
                EmergencyContactRelationship = patientDto.EmergencyContactRelationship,
                EmergencyContactPhone = patientDto.EmergencyContactPhone,
                BloodGroup = patientDto.BloodGroup,
                Allergies = patientDto.Allergies,
                InsuranceProvider = patientDto.InsuranceProvider,
                InsurancePolicyNumber = patientDto.InsurancePolicyNumber,
                GeneralNotes = patientDto.GeneralNotes
            };

            return View(updatePatientDto);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdatePatientDto updatePatientDto)
        {
            if (id != updatePatientDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = await _patientService.UpdatePatientAsync(updatePatientDto);
                if (success)
                {
                    TempData["SuccessMessage"] = "Patient details updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the patient. Please try again.");
            }
            return View(updatePatientDto);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }

            // Fetch appointments for this patient
            ViewBag.PatientAppointments = await _appointmentService.GetAppointmentsForPatientAsync(id.Value);

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _patientService.DeletePatientAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Patient deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting patient. It might be in use or an unexpected error occurred.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
