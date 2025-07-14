using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Application.Services.HR;
using UniCareERP.Application.Services; // For IRoleService

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,HRManager")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService; // To get roles for dropdown
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            IRoleService roleService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _roleService = roleService;
            _logger = logger;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();
            // A more detailed DTO would be better here, but for now this is fine
            return View(employee);
        }


        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            await PopulateRolesViewBag();
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto createDto)
        {
            if (ModelState.IsValid)
            {
                var createdEmployee = await _employeeService.CreateEmployeeAsync(createDto);
                if (createdEmployee != null)
                {
                    TempData["SuccessMessage"] = $"Employee {createdEmployee.FullName} ({createdEmployee.EmployeeCode}) created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // This error could be more specific if the service returned error details
                    ModelState.AddModelError(string.Empty, "Failed to create employee. The email may already exist, or another error occurred.");
                }
            }
            await PopulateRolesViewBag();
            return View(createDto);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var employee = await _employeeService.GetEmployeeForUpdateAsync(id.Value);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateEmployeeDto updateDto)
        {
            if (id != updateDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(updateDto);
                if (updatedEmployee != null)
                {
                    TempData["SuccessMessage"] = "Employee details updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
                }
            }
            return View(updateDto);
        }

        // POST: Employees/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var success = await _employeeService.DeactivateEmployeeAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Employee deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to deactivate employee.";
            }
            return RedirectToAction(nameof(Index));
        }

         // POST: Employees/Reactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            var success = await _employeeService.ReactivateEmployeeAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Employee reactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reactivate employee.";
            }
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateRolesViewBag()
        {
            var roles = await _roleService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
        }
    }
}
