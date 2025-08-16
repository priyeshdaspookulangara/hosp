using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Radiology;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Application.Services.Patients;

namespace UniCareERP.Web.Controllers
{
    public class RadiologyController : Controller
    {
        private readonly IRISService _risService;
        private readonly IPatientService _patientService;

        public RadiologyController(IRISService risService, IPatientService patientService)
        {
            _risService = risService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _risService.GetOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientService.GetAllPatientsAsync();
            ViewBag.TestTypes = await _risService.GetTestTypesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadiologyOrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                await _risService.CreateOrderAsync(orderDto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = await _patientService.GetAllPatientsAsync();
            ViewBag.TestTypes = await _risService.GetTestTypesAsync();
            return View(orderDto);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _risService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
