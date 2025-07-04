using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UniCareERP.Web.Controllers
{
    // [Authorize(Roles = "HRManager,Admin")] // Example
    public class HRController : Controller
    {
        // private readonly IHRService _hrService;
        // public HRController(IHRService hrService) { _hrService = hrService; }

        public IActionResult Index()
        {
            ViewBag.Message = "Human Resources Management - Placeholder";
            return View();
        }
    }
}
