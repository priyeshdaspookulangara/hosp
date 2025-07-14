using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Inventory;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Pharmacist,FinanceHead")]
    public class PurchaseOrdersController : Controller
    {
        private readonly IPurchaseOrderService _poService;
        private readonly IInventoryService _inventoryService; // For item list
        private readonly ILogger<PurchaseOrdersController> _logger;

        public PurchaseOrdersController(
            IPurchaseOrderService poService,
            IInventoryService inventoryService,
            ILogger<PurchaseOrdersController> logger)
        {
            _poService = poService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var pos = await _poService.GetAllPurchaseOrdersAsync();
            return View(pos);
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var po = await _poService.GetPurchaseOrderByIdAsync(id.Value);
            if (po == null) return NotFound();
            return View(po);
        }

        // GET: PurchaseOrders/Create
        public async Task<IActionResult> Create()
        {
            await PopulateInventoryItemsViewBag();
            return View(new CreatePurchaseOrderDto());
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto createDto)
        {
             createDto.Items.RemoveAll(item => item.QuantityOrdered == 0);
             if (ModelState.IsValid)
             {
                 var createdPo = await _poService.CreatePurchaseOrderAsync(createDto);
                 if (createdPo != null)
                 {
                     TempData["SuccessMessage"] = $"Purchase Order {createdPo.PurchaseOrderNumber} created successfully.";
                     return RedirectToAction(nameof(Details), new { id = createdPo.Id });
                 }
                 else
                 {
                     ModelState.AddModelError(string.Empty, "Failed to create Purchase Order.");
                 }
             }
             await PopulateInventoryItemsViewBag();
             return View(createDto);
        }

        // POST: PurchaseOrders/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var success = await _poService.ApprovePurchaseOrderAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Purchase Order approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve Purchase Order. It may not be in a pending state.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PurchaseOrders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var success = await _poService.CancelPurchaseOrderAsync(id);
             if (success)
            {
                TempData["SuccessMessage"] = "Purchase Order cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel Purchase Order. It may have already been partially received.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PurchaseOrders/ReceiveGoods/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveGoods(Guid id, List<ReceivedItemDto> items)
        {
            items.RemoveAll(i => i.QuantityReceived <= 0);
            if (!items.Any())
            {
                TempData["WarningMessage"] = "No items were marked as received.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var updatedPo = await _poService.ReceiveGoodsAsync(id, items);
            if (updatedPo != null)
            {
                TempData["SuccessMessage"] = "Goods received and stock levels updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to receive goods. Please check quantities and PO status.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }


        private async Task PopulateInventoryItemsViewBag()
        {
            var items = await _inventoryService.GetAllItemsAsync();
            // Filter for active items only for new POs
            ViewBag.InventoryItems = new SelectList(items.Where(i => i.IsActive).OrderBy(i => i.Name), "Id", "Name");
        }
    }
}
