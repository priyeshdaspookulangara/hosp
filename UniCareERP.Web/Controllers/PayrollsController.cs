using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniCareERP.Application.Services.HR;

namespace UniCareERP.Web.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly IPayrollService _payrollService;
        private readonly IEmployeeService _employeeService;

        public PayrollsController(IPayrollService payrollService, IEmployeeService employeeService)
        {
            _payrollService = payrollService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(Guid employeeId)
        {
            var payrolls = await _payrollService.GetPayrollsForEmployeeAsync(employeeId);
            return View(payrolls);
        }

        public async Task<IActionResult> Details(Guid payrollId)
        {
            var payroll = await _payrollService.GetPayrollByIdAsync(payrollId);
            if (payroll == null)
            {
                return NotFound();
            }
            return View(payroll);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(Guid employeeId, DateTime payPeriodStartDate, DateTime payPeriodEndDate)
        {
            var payroll = await _payrollService.GeneratePayrollAsync(employeeId, payPeriodStartDate, payPeriodEndDate);
            if (payroll == null)
            {
                return BadRequest("Could not generate payroll.");
            }
            return RedirectToAction(nameof(Details), new { payrollId = payroll.Id });
        }

        public async Task<IActionResult> Generate()
        {
            ViewBag.Employees = await _employeeService.GetAllEmployeesAsync();
            return View();
        }
    }
}
