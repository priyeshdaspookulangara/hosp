using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Pharmacy;
using UniCareERP.Application.Services.Pharmacy;

namespace UniCareERP.Web.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        public async Task<IActionResult> Index()
        {
            var drugs = await _pharmacyService.GetAllDrugsAsync();
            return View(drugs);
        }

        public async Task<IActionResult> Prescriptions()
        {
            var prescriptions = await _pharmacyService.GetAllPrescriptionsAsync();
            return View(prescriptions);
        }
    }
}
