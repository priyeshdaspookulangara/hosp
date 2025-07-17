using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Web.ViewModels.Finance;

namespace UniCareERP.Web.Controllers
{
    [Authorize(Roles = "Admin,FinanceHead,Receptionist")] // Example roles
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPatientService _patientService; // To get patient list
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(
            IInvoiceService invoiceService,
            IPatientService patientService,
            ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _patientService = patientService;
            _logger = logger;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return View(invoices);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id.Value);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create(Guid? patientId, Guid? appointmentId)
        {
            await PopulatePatientsViewBag(patientId);
            var model = new CreateInvoiceDto();
            if (patientId.HasValue)
            {
                model.PatientId = patientId.Value;
            }
            if (appointmentId.HasValue)
            {
                 model.SourceAppointmentId = appointmentId.Value;
                 // You could pre-fill items based on the appointment here if you had a service for it
                 // e.g., model.InvoiceItems.Add(new CreateInvoiceItemDto { Description = "Consultation Fee", ... });
            }
            return View(model);
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInvoiceDto createInvoiceDto)
        {
            // Remove empty items that might be submitted from the form
            createInvoiceDto.InvoiceItems.RemoveAll(item =>
                string.IsNullOrWhiteSpace(item.Description) && item.Quantity == 0 && item.UnitPrice == 0);

            if (ModelState.IsValid)
            {
                var createdInvoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto);
                if (createdInvoice != null)
                {
                    TempData["SuccessMessage"] = $"Invoice {createdInvoice.InvoiceNumber} created successfully.";
                    return RedirectToAction(nameof(Details), new { id = createdInvoice.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create invoice. Please check the details and try again.");
                }
            }
            await PopulatePatientsViewBag(createInvoiceDto.PatientId);
            return View(createInvoiceDto);
        }

        // POST: Invoices/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, InvoiceStatus status)
        {
            var success = await _invoiceService.UpdateInvoiceStatusAsync(id, status);
            if(success)
            {
                TempData["SuccessMessage"] = $"Invoice status updated to {status}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update invoice status.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Invoices/AddPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(AddPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid payment details.";
                return RedirectToAction(nameof(Details), new { id = model.InvoiceId });
            }

            var updatedInvoice = await _invoiceService.AddPaymentToInvoiceAsync(
                model.InvoiceId,
                model.Amount,
                DateTime.UtcNow,
                model.PaymentMethod,
                model.TransactionId,
                model.Notes);

            if (updatedInvoice != null)
            {
                TempData["SuccessMessage"] = $"Payment of {model.Amount:C} successfully added.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add payment.";
            }
            return RedirectToAction(nameof(Details), new { id = model.InvoiceId });
        }


        private async Task PopulatePatientsViewBag(Guid? selectedPatientId = null)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.PatientId = new SelectList(patients.OrderBy(p => p.FullName), "Id", "FullName", selectedPatientId);
        }
    }
}
