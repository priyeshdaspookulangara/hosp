using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Emergency;
using UniCareERP.Application.Services.Emergency;
using UniCareERP.Application.Services.Patients;

namespace UniCareERP.Web.Controllers
{
    public class EmergencyController : Controller
    {
        private readonly IEmergencyCaseService _emergencyCaseService;
        private readonly IPatientService _patientService;

        public EmergencyController(IEmergencyCaseService emergencyCaseService, IPatientService patientService)
        {
            _emergencyCaseService = emergencyCaseService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var emergencyCases = await _emergencyCaseService.GetAllEmergencyCasesAsync();
            return View(emergencyCases);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientService.GetAllPatientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmergencyCaseDto createEmergencyCaseDto)
        {
            if (ModelState.IsValid)
            {
                await _emergencyCaseService.CreateEmergencyCaseAsync(createEmergencyCaseDto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = await _patientService.GetAllPatientsAsync();
            return View(createEmergencyCaseDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var emergencyCase = await _emergencyCaseService.GetEmergencyCaseByIdAsync(id);
            if (emergencyCase == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateEmergencyCaseDto
            {
                Id = emergencyCase.Id,
                CaseDescription = emergencyCase.CaseDescription,
                Status = emergencyCase.Status
            };

            ViewBag.PatientName = emergencyCase.PatientName;

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateEmergencyCaseDto updateEmergencyCaseDto)
        {
            if (id != updateEmergencyCaseDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _emergencyCaseService.UpdateEmergencyCaseAsync(updateEmergencyCaseDto);
                return RedirectToAction(nameof(Index));
            }
            return View(updateEmergencyCaseDto);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var emergencyCase = await _emergencyCaseService.GetEmergencyCaseByIdAsync(id);
            if (emergencyCase == null)
            {
                return NotFound();
            }
            return View(emergencyCase);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var emergencyCase = await _emergencyCaseService.GetEmergencyCaseByIdAsync(id);
            if (emergencyCase == null)
            {
                return NotFound();
            }
            return View(emergencyCase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _emergencyCaseService.DeleteEmergencyCaseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
