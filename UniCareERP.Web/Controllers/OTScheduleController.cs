using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.OperationTheatre;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Web.Controllers
{
    public class OTScheduleController : Controller
    {
        private readonly IOTScheduleService _otScheduleService;

        public OTScheduleController(IOTScheduleService otScheduleService)
        {
            _otScheduleService = otScheduleService;
        }

        public async Task<IActionResult> Index()
        {
            var otSchedules = await _otScheduleService.GetAllOTSchedulesAsync();
            return View(otSchedules);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var otSchedule = await _otScheduleService.GetOTScheduleByIdAsync(id);
            if (otSchedule == null)
            {
                return NotFound();
            }
            return View(otSchedule);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOTScheduleDto createOTScheduleDto)
        {
            if (ModelState.IsValid)
            {
                await _otScheduleService.CreateOTScheduleAsync(createOTScheduleDto);
                return RedirectToAction(nameof(Index));
            }
            return View(createOTScheduleDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var otSchedule = await _otScheduleService.GetOTScheduleByIdAsync(id);
            if (otSchedule == null)
            {
                return NotFound();
            }
            var updateOTScheduleDto = new UpdateOTScheduleDto
            {
                Id = otSchedule.Id,
                OperationTheatreId = otSchedule.OperationTheatreId,
                SurgicalProcedureId = otSchedule.SurgicalProcedureId,
                PatientId = otSchedule.PatientId,
                SurgicalTeamId = otSchedule.SurgicalTeamId,
                StartTime = otSchedule.StartTime,
                EndTime = otSchedule.EndTime,
                Notes = otSchedule.Notes
            };
            return View(updateOTScheduleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateOTScheduleDto updateOTScheduleDto)
        {
            if (id != updateOTScheduleDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _otScheduleService.UpdateOTScheduleAsync(updateOTScheduleDto);
                return RedirectToAction(nameof(Index));
            }
            return View(updateOTScheduleDto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var otSchedule = await _otScheduleService.GetOTScheduleByIdAsync(id);
            if (otSchedule == null)
            {
                return NotFound();
            }
            return View(otSchedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _otScheduleService.DeleteOTScheduleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
