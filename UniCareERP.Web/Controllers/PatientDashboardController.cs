using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.Services.PatientDashboard;

namespace UniCareERP.Web.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly IPatientDashboardService _patientDashboardService;

        public PatientDashboardController(IPatientDashboardService patientDashboardService)
        {
            _patientDashboardService = patientDashboardService;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var dashboardData = await _patientDashboardService.GetPatientDashboardAsync(id);
            if (dashboardData == null)
            {
                return NotFound();
            }
            return View(dashboardData);
        }
    }
}
