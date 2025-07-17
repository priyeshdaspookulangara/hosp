using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using UniCareERP.Application.Services.Finance;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "FinanceHead,Admin")]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _financeService.GetFinanceDashboardDataAsync();
            return View(dashboardData);
        }
    }
}
