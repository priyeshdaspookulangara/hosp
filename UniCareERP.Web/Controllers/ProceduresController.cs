using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Procedures;
using UniCareERP.Application.Services.Procedures;

namespace UniCareERP.Web.Controllers
{
    public class ProceduresController : Controller
    {
        private readonly IProcedureService _procedureService;

        public ProceduresController(IProcedureService procedureService)
        {
            _procedureService = procedureService;
        }

        public async Task<IActionResult> Index()
        {
            var procedures = await _procedureService.GetAllProceduresAsync();
            return View(procedures);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var procedure = await _procedureService.GetProcedureByIdAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }
            return View(procedure);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProcedureDto createProcedureDto)
        {
            if (ModelState.IsValid)
            {
                await _procedureService.CreateProcedureAsync(createProcedureDto);
                return RedirectToAction(nameof(Index));
            }
            return View(createProcedureDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var procedure = await _procedureService.GetProcedureByIdAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }
            // Note: A mapping from ProcedureDto to UpdateProcedureDto would be needed here.
            // For simplicity, we'll assume a direct mapping or use a different view model.
            return View(procedure);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateProcedureDto updateProcedureDto)
        {
            if (id != updateProcedureDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _procedureService.UpdateProcedureAsync(updateProcedureDto);
                return RedirectToAction(nameof(Index));
            }
            return View(updateProcedureDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _procedureService.DeleteProcedureAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
