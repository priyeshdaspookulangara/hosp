using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Vitals;
using UniCareERP.Application.Services.Vitals;

namespace UniCareERP.Web.Controllers
{
    public class VitalsController : Controller
    {
        private readonly IVitalService _vitalService;

        public VitalsController(IVitalService vitalService)
        {
            _vitalService = vitalService;
        }

        public async Task<IActionResult> Index(Guid patientId)
        {
            var vitals = await _vitalService.GetVitalsForPatientAsync(patientId);
            return View(vitals);
        }

        public IActionResult Create(Guid patientId)
        {
            var vitalDto = new VitalDto { PatientId = patientId };
            return View(vitalDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VitalDto vitalDto)
        {
            if (ModelState.IsValid)
            {
                // Assuming the logged-in user's ID is available
                // vitalDto.RecordedById = ...;
                await _vitalService.CreateVitalAsync(vitalDto);
                return RedirectToAction(nameof(Index), new { patientId = vitalDto.PatientId });
            }
            return View(vitalDto);
        }
    }
}
