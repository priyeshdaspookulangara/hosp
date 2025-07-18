using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.Services.Reporting;

namespace UniCareERP.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportingService _reportingService;

        public ReportsController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        public async Task<IActionResult> Procedures(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now;
            var start = startDate ?? end.AddMonths(-1);
            ViewData["StartDate"] = start;
            ViewData["EndDate"] = end;
            var report = await _reportingService.GetProcedureReportAsync(start, end);
            return View(report);
        }

        public async Task<IActionResult> Finance(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now;
            var start = startDate ?? end.AddMonths(-1);
            ViewData["StartDate"] = start;
            ViewData["EndDate"] = end;
            var report = await _reportingService.GetFinanceReportAsync(start, end);
            return View(report);
        }
    }
}
