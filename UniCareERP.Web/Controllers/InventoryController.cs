using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UniCareERP.Web.Controllers
{
    // [Authorize(Roles = "Pharmacist,Admin")] // Example
    public class InventoryController : Controller
    {
        // private readonly IInventoryService _inventoryService;
        // public InventoryController(IInventoryService inventoryService) { _inventoryService = inventoryService; }

        public IActionResult Index()
        {
            ViewBag.Message = "Medical Shop / Inventory Management - Placeholder";
            return View();
        }
    }
}
