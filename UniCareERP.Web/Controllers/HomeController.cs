using Microsoft.AspNetCore.Authorization; // To secure the dashboard
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using UniCareERP.Application.Services.Dashboard;

namespace UniCareERP.Web.Controllers;

[Authorize] // The dashboard should require a login
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync();
        return View(dashboardData);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    #region API Methods for Dashboard Charts
    [HttpGet]
    public async Task<IActionResult> GetWeeklySalesChartData()
    {
        // In a real app, you might get this from a more specialized service method
        var data = (await _dashboardService.GetDashboardDataAsync()).WeeklySalesChart;
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointmentStatusChartData()
    {
        var data = (await _dashboardService.GetDashboardDataAsync()).AppointmentStatusChart;
        return Json(data);
    }
    #endregion

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
