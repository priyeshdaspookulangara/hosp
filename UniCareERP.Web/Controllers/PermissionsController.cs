using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Permissions;
using UniCareERP.Application.Services.Permissions;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissionsController : Controller
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index(string roleId)
        {
            var permissions = await _permissionService.GetPermissionsForRoleAsync(roleId);
            ViewBag.RoleId = roleId;
            return View(permissions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdatePermissionsDto updatePermissionsDto)
        {
            if (ModelState.IsValid)
            {
                await _permissionService.UpdatePermissionsForRoleAsync(updatePermissionsDto);
                return RedirectToAction(nameof(Index), new { roleId = updatePermissionsDto.RoleId });
            }

            return View("Index", updatePermissionsDto.Permissions);
        }
    }
}
