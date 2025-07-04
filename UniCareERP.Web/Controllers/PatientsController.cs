using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // If needed later
// using UniCareERP.Application.Services.Patients; // When service is implemented

namespace UniCareERP.Web.Controllers
{
    // [Authorize] // Example, adjust authorization as needed
    public class PatientsController : Controller
    {
        // private readonly IPatientService _patientService;

        // public PatientsController(IPatientService patientService)
        // {
        //    _patientService = patientService;
        // }

        // GET: Patients
        public IActionResult Index()
        {
            // var patients = await _patientService.GetAllPatientsAsync();
            // return View(patients);
            ViewBag.Message = "Patient Management - Placeholder";
            return View();
        }
    }
}
