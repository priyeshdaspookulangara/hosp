using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Web.Controllers.Radiology
{
    public class WorklistItemController : Controller
    {
        private readonly IWorklistItemService _worklistItemService;

        public WorklistItemController(IWorklistItemService worklistItemService)
        {
            _worklistItemService = worklistItemService;
        }

        public async Task<IActionResult> Index()
        {
            var worklistItems = await _worklistItemService.GetAllWorklistItemsAsync();
            return View(worklistItems);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var worklistItem = await _worklistItemService.GetWorklistItemByIdAsync(id);
            if (worklistItem == null)
            {
                return NotFound();
            }
            return View(worklistItem);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorklistItem worklistItem)
        {
            if (ModelState.IsValid)
            {
                await _worklistItemService.AddWorklistItemAsync(worklistItem);
                return RedirectToAction(nameof(Index));
            }
            return View(worklistItem);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var worklistItem = await _worklistItemService.GetWorklistItemByIdAsync(id);
            if (worklistItem == null)
            {
                return NotFound();
            }
            return View(worklistItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, WorklistItem worklistItem)
        {
            if (id != worklistItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _worklistItemService.UpdateWorklistItemAsync(worklistItem);
                return RedirectToAction(nameof(Index));
            }
            return View(worklistItem);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var worklistItem = await _worklistItemService.GetWorklistItemByIdAsync(id);
            if (worklistItem == null)
            {
                return NotFound();
            }
            return View(worklistItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _worklistItemService.DeleteWorklistItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
