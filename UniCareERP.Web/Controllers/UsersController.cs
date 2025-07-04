using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniCareERP.Application.Services;
using UniCareERP.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllRoles = new SelectList(await _roleService.GetAllRolesAsync(), "Name", "Name");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto createUserDto)
        {
            if (ModelState.IsValid)
            {
                var (success, userId, errors) = await _userService.CreateUserAsync(createUserDto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            ViewBag.AllRoles = new SelectList(await _roleService.GetAllRolesAsync(), "Name", "Name", createUserDto.Roles);
            return View(createUserDto);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userService.GetUserForUpdateAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.AllRoles = new SelectList(await _roleService.GetAllRolesAsync(), "Name", "Name", user.Roles);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateUserDto updateUserDto)
        {
            if (id != updateUserDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var (success, errors) = await _userService.UpdateUserAsync(updateUserDto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            ViewBag.AllRoles = new SelectList(await _roleService.GetAllRolesAsync(), "Name", "Name", updateUserDto.Roles);
            return View(updateUserDto);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var (success, errors) = await _userService.DeleteUserAsync(id);
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
