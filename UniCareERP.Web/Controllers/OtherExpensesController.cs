using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Web.Controllers
{
    public class OtherExpensesController : Controller
    {
        private readonly IOtherExpenseService _otherExpenseService;
        private readonly UniCareDbContext _context;

        public OtherExpensesController(IOtherExpenseService otherExpenseService, UniCareDbContext context)
        {
            _otherExpenseService = otherExpenseService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _otherExpenseService.GetAllExpensesAsync();
            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateGlAccountsViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOtherExpenseDto createDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _otherExpenseService.CreateExpenseAsync(createDto);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "An error occurred while creating the expense.");
            }
            await PopulateGlAccountsViewBag();
            return View(createDto);
        }

        private async Task PopulateGlAccountsViewBag()
        {
            ViewBag.GeneralLedgerAccounts = new SelectList(await _context.GeneralLedgerAccounts.Where(a => a.IsActive).ToListAsync(), "Id", "AccountName");
        }
    }
}
