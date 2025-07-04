using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UniCareERP.Web.Controllers
{
    // [Authorize(Roles = "FinanceHead,Admin")] // Example
    public class FinanceController : Controller
    {
        // private readonly IFinanceService _financeService;
        // public FinanceController(IFinanceService financeService) { _financeService = financeService; }

        public IActionResult Index()
        {
            ViewBag.Message = "Finance Management - Placeholder";
            return View();
        }
    }
}
