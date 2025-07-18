using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;
using UniCareERP.Application.Services.Lab;

namespace UniCareERP.Web.Controllers
{
    public class LabController : Controller
    {
        private readonly ILabService _labService;

        public LabController(ILabService labService)
        {
            _labService = labService;
        }

        // GET: Lab/LabTests
        public async Task<IActionResult> Index()
        {
            var labTests = await _labService.GetAllLabTestsAsync();
            return View(labTests);
        }

        // GET: Lab/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var labTest = await _labService.GetLabTestByIdAsync(id);
            if (labTest == null)
            {
                return NotFound();
            }
            return View(labTest);
        }

        // GET: Lab/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lab/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabTestDto labTestDto)
        {
            if (ModelState.IsValid)
            {
                await _labService.CreateLabTestAsync(labTestDto);
                return RedirectToAction(nameof(Index));
            }
            return View(labTestDto);
        }

        // GET: Lab/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var labTest = await _labService.GetLabTestByIdAsync(id);
            if (labTest == null)
            {
                return NotFound();
            }
            return View(labTest);
        }

        // POST: Lab/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LabTestDto labTestDto)
        {
            if (id != labTestDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _labService.UpdateLabTestAsync(id, labTestDto);
                return RedirectToAction(nameof(Index));
            }
            return View(labTestDto);
        }

        // GET: Lab/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var labTest = await _labService.GetLabTestByIdAsync(id);
            if (labTest == null)
            {
                return NotFound();
            }
            return View(labTest);
        }

        // POST: Lab/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _labService.DeleteLabTestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Lab/CreateOrder
        public async Task<IActionResult> CreateOrder(Guid patientId)
        {
            ViewBag.LabTests = await _labService.GetAllLabTestsAsync();
            var labOrderDto = new LabOrderDto
            {
                PatientId = patientId
            };
            return View(labOrderDto);
        }

        // POST: Lab/CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(LabOrderDto labOrderDto)
        {
            if (ModelState.IsValid)
            {
                await _labService.CreateLabOrderAsync(labOrderDto);
                return RedirectToAction("Details", "Patients", new { id = labOrderDto.PatientId });
            }
            ViewBag.LabTests = await _labService.GetAllLabTestsAsync();
            return View(labOrderDto);
        }
    }
}
