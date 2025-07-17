using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Web.Controllers.Radiology
{
    public class RadiologyReportController : Controller
    {
        private readonly IRadiologyReportService _radiologyReportService;

        public RadiologyReportController(IRadiologyReportService radiologyReportService)
        {
            _radiologyReportService = radiologyReportService;
        }

        public async Task<IActionResult> Index()
        {
            var radiologyReports = await _radiologyReportService.GetAllRadiologyReportsAsync();
            return View(radiologyReports);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var radiologyReport = await _radiologyReportService.GetRadiologyReportByIdAsync(id);
            if (radiologyReport == null)
            {
                return NotFound();
            }
            return View(radiologyReport);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadiologyReport radiologyReport)
        {
            if (ModelState.IsValid)
            {
                await _radiologyReportService.AddRadiologyReportAsync(radiologyReport);
                return RedirectToAction(nameof(Index));
            }
            return View(radiologyReport);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var radiologyReport = await _radiologyReportService.GetRadiologyReportByIdAsync(id);
            if (radiologyReport == null)
            {
                return NotFound();
            }
            return View(radiologyReport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RadiologyReport radiologyReport)
        {
            if (id != radiologyReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _radiologyReportService.UpdateRadiologyReportAsync(radiologyReport);
                return RedirectToAction(nameof(Index));
            }
            return View(radiologyReport);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var radiologyReport = await _radiologyReportService.GetRadiologyReportByIdAsync(id);
            if (radiologyReport == null)
            {
                return NotFound();
            }
            return View(radiologyReport);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _radiologyReportService.DeleteRadiologyReportAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
