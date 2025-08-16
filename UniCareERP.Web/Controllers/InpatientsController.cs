using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Inpatient;
using UniCareERP.Application.DTOs.Inpatient;

namespace UniCareERP.Web.Controllers
{
    public class InpatientsController : Controller
    {
        private readonly IInpatientService _inpatientService;

        public InpatientsController(IInpatientService inpatientService)
        {
            _inpatientService = inpatientService;
        }

        public async Task<IActionResult> Index()
        {
            var inpatients = await _inpatientService.GetAllInpatientsAsync();
            return View(inpatients);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var inpatient = await _inpatientService.GetInpatientByIdAsync(id);
            if (inpatient == null)
            {
                return NotFound();
            }
            return View(inpatient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInpatientDto createInpatientDto)
        {
            if (ModelState.IsValid)
            {
                await _inpatientService.CreateInpatientAsync(createInpatientDto);
                return RedirectToAction(nameof(Index));
            }
            return View(createInpatientDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var inpatient = await _inpatientService.GetInpatientByIdAsync(id);
            if (inpatient == null)
            {
                return NotFound();
            }
            var updateInpatientDto = new UpdateInpatientDto
            {
                Id = inpatient.Id,
                PatientId = inpatient.PatientId,
                AdmissionDate = inpatient.AdmissionDate,
                DischargeDate = inpatient.DischargeDate,
                RoomNumber = inpatient.RoomNumber,
                BedNumber = inpatient.BedNumber,
                AdmissionReason = inpatient.AdmissionReason,
                DischargeReason = inpatient.DischargeReason
            };
            return View(updateInpatientDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateInpatientDto updateInpatientDto)
        {
            if (id != updateInpatientDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _inpatientService.UpdateInpatientAsync(updateInpatientDto);
                return RedirectToAction(nameof(Index));
            }
            return View(updateInpatientDto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var inpatient = await _inpatientService.GetInpatientByIdAsync(id);
            if (inpatient == null)
            {
                return NotFound();
            }
            return View(inpatient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _inpatientService.DeleteInpatientAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
