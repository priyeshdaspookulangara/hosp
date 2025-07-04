using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services;
using UniCareERP.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleDto createRoleDto)
        {
            if (ModelState.IsValid)
            {
                var (success, roleId, errors) = await _roleService.CreateRoleAsync(createRoleDto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            return View(createRoleDto);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            // Map to UpdateRoleDto
            var updateRoleDto = new UpdateRoleDto { Id = role.Id, Name = role.Name, Description = role.Description };
            return View(updateRoleDto);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateRoleDto updateRoleDto)
        {
            if (id != updateRoleDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var (success, errors) = await _roleService.UpdateRoleAsync(updateRoleDto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            return View(updateRoleDto);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
             var (success, errors) = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                // Handle error, maybe add to TempData or ModelState
                TempData["ErrorMessage"] = string.Join(", ", errors);
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
