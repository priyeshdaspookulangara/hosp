using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Inventory;
using UniCareERP.Application.Services.Patients;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Pharmacist,Receptionist")]
    public class SalesController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IInventoryService _inventoryService;
        private readonly IPatientService _patientService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(
            ISaleService saleService,
            IInventoryService inventoryService,
            IPatientService patientService,
            ILogger<SalesController> logger)
        {
            _saleService = saleService;
            _inventoryService = inventoryService;
            _patientService = patientService;
            _logger = logger;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return View(sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var sale = await _saleService.GetSaleByIdAsync(id.Value);
            if (sale == null) return NotFound();
            return View(sale);
        }


        // GET: Sales/PointOfSale
        public async Task<IActionResult> PointOfSale()
        {
            await PopulatePatientsViewBag();
            return View();
        }

        // POST: Sales/CreateSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSale(CreateSaleDto createDto)
        {
            if (createDto.Items == null || !createDto.Items.Any())
            {
                ModelState.AddModelError("", "Cannot create a sale with no items.");
            }

            if (ModelState.IsValid)
            {
                var result = await _saleService.CreateSaleAsync(createDto);
                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Sale {result.SaleNumber} completed successfully.";
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = result.Id }) });
                }
                else
                {
                    // This error is generic. A more robust implementation would return specific error messages.
                    return Json(new { success = false, message = "Sale failed. One or more items may be out of stock." });
                }
            }

            // If model state is invalid, collect errors and return
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Invalid data submitted.", errors = errors });
        }


        // API endpoint for item search used by POS view
        [HttpGet]
        public async Task<IActionResult> SearchInventoryItems(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var allItems = await _inventoryService.GetAllItemsAsync();
            var filteredItems = allItems
                .Where(i => i.IsActive &&
                            (i.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                             i.ItemCode.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(i => new {
                    id = i.Id,
                    label = $"{i.Name} ({i.ItemCode}) - Stock: {i.QuantityInStock}", // For jQuery UI Autocomplete
                    value = i.Name, // For jQuery UI Autocomplete
                    price = i.UnitPrice,
                    stock = i.QuantityInStock
                })
                .Take(10); // Limit results

            return Json(filteredItems);
        }

        private async Task PopulatePatientsViewBag(Guid? selectedPatientId = null)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.PatientId = new SelectList(patients.OrderBy(p => p.FullName), "Id", "FullName", selectedPatientId);
        }
    }
}
