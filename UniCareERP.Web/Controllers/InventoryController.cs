using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Inventory;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Pharmacist,FinanceHead")] // Example roles
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var items = await _inventoryService.GetAllItemsAsync();
            return View(items);
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var item = await _inventoryService.GetItemByIdAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: Inventory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInventoryItemDto createDto)
        {
            if (ModelState.IsValid)
            {
                var createdItem = await _inventoryService.CreateItemAsync(createDto);
                if (createdItem != null)
                {
                    TempData["SuccessMessage"] = $"Item '{createdItem.Name}' ({createdItem.ItemCode}) created successfully. Adjust stock to add initial quantity.";
                    return RedirectToAction(nameof(Details), new { id = createdItem.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create item.");
                }
            }
            return View(createDto);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var itemDto = await _inventoryService.GetItemByIdAsync(id.Value);
            if (itemDto == null) return NotFound();

            var updateDto = new UpdateInventoryItemDto
            {
                Id = itemDto.Id,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Category = itemDto.Category,
                UnitOfMeasure = itemDto.UnitOfMeasure,
                UnitPrice = itemDto.UnitPrice,
                CostPrice = itemDto.CostPrice,
                ReorderLevel = itemDto.ReorderLevel,
                SupplierInfo = itemDto.SupplierInfo,
                ExpiryDate = itemDto.ExpiryDate,
                BatchNumber = itemDto.BatchNumber,
                IsActive = itemDto.IsActive
            };
            return View(updateDto);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateInventoryItemDto updateDto)
        {
            if (id != updateDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var updatedItem = await _inventoryService.UpdateItemAsync(updateDto);
                if (updatedItem != null)
                {
                    TempData["SuccessMessage"] = "Item details updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = updatedItem.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update item.");
                }
            }
            return View(updateDto);
        }

        // POST: Inventory/Delete/5 (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _inventoryService.DeleteItemAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Item deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to deactivate item.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Inventory/AdjustStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(Guid id, int quantity, string reason)
        {
            if (quantity == 0)
            {
                 TempData["WarningMessage"] = "Quantity change cannot be zero.";
                 return RedirectToAction(nameof(Details), new { id });
            }

            var success = await _inventoryService.AdjustStockAsync(id, quantity, reason);
             if (success)
            {
                TempData["SuccessMessage"] = $"Stock adjusted by {quantity} successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to adjust stock. Insufficient quantity or other error.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
