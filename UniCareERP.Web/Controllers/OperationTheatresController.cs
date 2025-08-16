using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.OperationTheatre;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Web.Controllers
{
    public class OperationTheatresController : Controller
    {
        private readonly IOperationTheatreService _operationTheatreService;

        public OperationTheatresController(IOperationTheatreService operationTheatreService)
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
        public async Task<IActionResult> Create(CreateOperationTheatreDto createOperationTheatreDto)
        {
            if (ModelState.IsValid)
            {
                await _operationTheatreService.CreateOperationTheatreAsync(createOperationTheatreDto);
                return RedirectToAction(nameof(Index));
            }
            return View(createOperationTheatreDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var operationTheatre = await _operationTheatreService.GetOperationTheatreByIdAsync(id);
            if (operationTheatre == null)
            {
                return NotFound();
            }
            var updateOperationTheatreDto = new UpdateOperationTheatreDto
            {
                Id = operationTheatre.Id,
                Name = operationTheatre.Name,
                RoomNumber = operationTheatre.RoomNumber,
                IsAvailable = operationTheatre.IsAvailable,
                Location = operationTheatre.Location,
                Equipment = operationTheatre.Equipment
            };
            return View(updateOperationTheatreDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateOperationTheatreDto updateOperationTheatreDto)
        {
            if (id != updateOperationTheatreDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _operationTheatreService.UpdateOperationTheatreAsync(updateOperationTheatreDto);
                return RedirectToAction(nameof(Index));
            }
            return View(updateOperationTheatreDto);
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

        public async Task<IActionResult> SurgicalProcedures()
        {
            var surgicalProcedures = await _operationTheatreService.GetAllSurgicalProceduresAsync();
            return View(surgicalProcedures);
        }

        public async Task<IActionResult> SurgicalProcedureDetails(Guid id)
        {
            var surgicalProcedure = await _operationTheatreService.GetSurgicalProcedureByIdAsync(id);
            if (surgicalProcedure == null)
            {
                return NotFound();
            }
            return View(surgicalProcedure);
        }

        public IActionResult CreateSurgicalProcedure()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSurgicalProcedure(CreateSurgicalProcedureDto createSurgicalProcedureDto)
        {
            if (ModelState.IsValid)
            {
                await _operationTheatreService.CreateSurgicalProcedureAsync(createSurgicalProcedureDto);
                return RedirectToAction(nameof(SurgicalProcedures));
            }
            return View(createSurgicalProcedureDto);
        }

        public async Task<IActionResult> EditSurgicalProcedure(Guid id)
        {
            var surgicalProcedure = await _operationTheatreService.GetSurgicalProcedureByIdAsync(id);
            if (surgicalProcedure == null)
            {
                return NotFound();
            }
            var updateSurgicalProcedureDto = new UpdateSurgicalProcedureDto
            {
                Id = surgicalProcedure.Id,
                Name = surgicalProcedure.Name,
                Description = surgicalProcedure.Description,
                RequiredEquipment = surgicalProcedure.RequiredEquipment,
                DurationMinutes = surgicalProcedure.DurationMinutes
            };
            return View(updateSurgicalProcedureDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSurgicalProcedure(Guid id, UpdateSurgicalProcedureDto updateSurgicalProcedureDto)
        {
            if (id != updateSurgicalProcedureDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _operationTheatreService.UpdateSurgicalProcedureAsync(updateSurgicalProcedureDto);
                return RedirectToAction(nameof(SurgicalProcedures));
            }
            return View(updateSurgicalProcedureDto);
        }

        public async Task<IActionResult> DeleteSurgicalProcedure(Guid id)
        {
            var surgicalProcedure = await _operationTheatreService.GetSurgicalProcedureByIdAsync(id);
            if (surgicalProcedure == null)
            {
                return NotFound();
            }
            return View(surgicalProcedure);
        }

        [HttpPost, ActionName("DeleteSurgicalProcedure")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurgicalProcedureConfirmed(Guid id)
        {
            await _operationTheatreService.DeleteSurgicalProcedureAsync(id);
            return RedirectToAction(nameof(SurgicalProcedures));
        }

        public async Task<IActionResult> SurgicalTeams()
        {
            var surgicalTeams = await _operationTheatreService.GetAllSurgicalTeamsAsync();
            return View(surgicalTeams);
        }

        public async Task<IActionResult> SurgicalTeamDetails(Guid id)
        {
            var surgicalTeam = await _operationTheatreService.GetSurgicalTeamByIdAsync(id);
            if (surgicalTeam == null)
            {
                return NotFound();
            }
            return View(surgicalTeam);
        }

        public IActionResult CreateSurgicalTeam()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSurgicalTeam(CreateSurgicalTeamDto createSurgicalTeamDto)
        {
            if (ModelState.IsValid)
            {
                await _operationTheatreService.CreateSurgicalTeamAsync(createSurgicalTeamDto);
                return RedirectToAction(nameof(SurgicalTeams));
            }
            return View(createSurgicalTeamDto);
        }

        public async Task<IActionResult> EditSurgicalTeam(Guid id)
        {
            var surgicalTeam = await _operationTheatreService.GetSurgicalTeamByIdAsync(id);
            if (surgicalTeam == null)
            {
                return NotFound();
            }
            var updateSurgicalTeamDto = new UpdateSurgicalTeamDto
            {
                Id = surgicalTeam.Id,
                Name = surgicalTeam.Name,
                MemberIds = surgicalTeam.Members.Select(m => m.Id)
            };
            return View(updateSurgicalTeamDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSurgicalTeam(Guid id, UpdateSurgicalTeamDto updateSurgicalTeamDto)
        {
            if (id != updateSurgicalTeamDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _operationTheatreService.UpdateSurgicalTeamAsync(updateSurgicalTeamDto);
                return RedirectToAction(nameof(SurgicalTeams));
            }
            return View(updateSurgicalTeamDto);
        }

        public async Task<IActionResult> DeleteSurgicalTeam(Guid id)
        {
            var surgicalTeam = await _operationTheatreService.GetSurgicalTeamByIdAsync(id);
            if (surgicalTeam == null)
            {
                return NotFound();
            }
            return View(surgicalTeam);
        }

        [HttpPost, ActionName("DeleteSurgicalTeam")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurgicalTeamConfirmed(Guid id)
        {
            await _operationTheatreService.DeleteSurgicalTeamAsync(id);
            return RedirectToAction(nameof(SurgicalTeams));
        }
    }
}
