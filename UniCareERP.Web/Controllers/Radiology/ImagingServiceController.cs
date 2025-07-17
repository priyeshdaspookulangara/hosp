using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Web.Controllers.Radiology
{
    public class ImagingServiceController : Controller
    {
        private readonly IImagingService _imagingService;

        public ImagingServiceController(IImagingService imagingService)
        {
            _imagingService = imagingService;
        }

        public async Task<IActionResult> Index()
        {
            var imagingServices = await _imagingService.GetAllImagingServicesAsync();
            return View(imagingServices);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var imagingService = await _imagingService.GetImagingServiceByIdAsync(id);
            if (imagingService == null)
            {
                return NotFound();
            }
            return View(imagingService);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImagingService imagingService)
        {
            if (ModelState.IsValid)
            {
                await _imagingService.AddImagingServiceAsync(imagingService);
                return RedirectToAction(nameof(Index));
            }
            return View(imagingService);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var imagingService = await _imagingService.GetImagingServiceByIdAsync(id);
            if (imagingService == null)
            {
                return NotFound();
            }
            return View(imagingService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ImagingService imagingService)
        {
            if (id != imagingService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _imagingService.UpdateImagingServiceAsync(imagingService);
                return RedirectToAction(nameof(Index));
            }
            return View(imagingService);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var imagingService = await _imagingService.GetImagingServiceByIdAsync(id);
            if (imagingService == null)
            {
                return NotFound();
            }
            return View(imagingService);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _imagingService.DeleteImagingServiceAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
