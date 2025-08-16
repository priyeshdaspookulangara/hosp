using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;
using UniCareERP.Application.Services.OperationTheatre;

namespace UniCareERP.Web.Controllers
{
    public class OperationTheatreController : Controller
    {
        private readonly IOperationTheatreService _operationTheatreService;

        public OperationTheatreController(IOperationTheatreService operationTheatreService)
        {
            _operationTheatreService = operationTheatreService;
        }

        public async Task<IActionResult> Index()
        {
            var operationTheatres = await _operationTheatreService.GetAllOperationTheatresAsync();
            return View(operationTheatres);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var operationTheatre = await _operationTheatreService.GetOperationTheatreByIdAsync(id);
            if (operationTheatre == null)
            {
                return NotFound();
            }
            return View(operationTheatre);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationTheatreDto operationTheatreDto)
        {
            if (ModelState.IsValid)
            {
                await _operationTheatreService.CreateOperationTheatreAsync(operationTheatreDto);
                return RedirectToAction(nameof(Index));
            }
            return View(operationTheatreDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var operationTheatre = await _operationTheatreService.GetOperationTheatreByIdAsync(id);
            if (operationTheatre == null)
            {
                return NotFound();
            }
            return View(operationTheatre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, OperationTheatreDto operationTheatreDto)
        {
            if (id != operationTheatreDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _operationTheatreService.UpdateOperationTheatreAsync(id, operationTheatreDto);
                return RedirectToAction(nameof(Index));
            }
            return View(operationTheatreDto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var operationTheatre = await _operationTheatreService.GetOperationTheatreByIdAsync(id);
            if (operationTheatre == null)
            {
                return NotFound();
            }
            return View(operationTheatre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _operationTheatreService.DeleteOperationTheatreAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
