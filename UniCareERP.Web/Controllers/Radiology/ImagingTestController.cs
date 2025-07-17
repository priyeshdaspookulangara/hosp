using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Web.Controllers.Radiology
{
    public class ImagingTestController : Controller
    {
        private readonly IImagingTestService _imagingTestService;

        public ImagingTestController(IImagingTestService imagingTestService)
        {
            _imagingTestService = imagingTestService;
        }

        public async Task<IActionResult> Index()
        {
            var imagingTests = await _imagingTestService.GetAllImagingTestsAsync();
            return View(imagingTests);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var imagingTest = await _imagingTestService.GetImagingTestByIdAsync(id);
            if (imagingTest == null)
            {
                return NotFound();
            }
            return View(imagingTest);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImagingTest imagingTest)
        {
            if (ModelState.IsValid)
            {
                await _imagingTestService.AddImagingTestAsync(imagingTest);
                return RedirectToAction(nameof(Index));
            }
            return View(imagingTest);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var imagingTest = await _imagingTestService.GetImagingTestByIdAsync(id);
            if (imagingTest == null)
            {
                return NotFound();
            }
            return View(imagingTest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ImagingTest imagingTest)
        {
            if (id != imagingTest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _imagingTestService.UpdateImagingTestAsync(imagingTest);
                return RedirectToAction(nameof(Index));
            }
            return View(imagingTest);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var imagingTest = await _imagingTestService.GetImagingTestByIdAsync(id);
            if (imagingTest == null)
            {
                return NotFound();
            }
            return View(imagingTest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _imagingTestService.DeleteImagingTestAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
