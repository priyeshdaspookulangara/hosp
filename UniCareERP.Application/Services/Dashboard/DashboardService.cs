using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Dashboard;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(UniCareDbContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            try
            {
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);

                // --- KPI Stats ---
                var statsDto = new DashboardStatsDto
                {
                    TotalActivePatients = await _context.Patients.CountAsync(p => p.RegistrationDate <= today), // Simplified logic
                    AppointmentsToday = await _context.Appointments.CountAsync(a => a.AppointmentDateTime.Date == today && a.Status != AppointmentStatus.CancelledByClinic && a.Status != AppointmentStatus.CancelledByPatient),
                    TotalSalesToday = await _context.Sales.Where(s => s.SaleDate.Date == today).SumAsync(s => s.TotalAmount),
                    LowStockItems = await _context.InventoryItems.CountAsync(i => i.IsActive && i.QuantityInStock <= i.ReorderLevel),
                    PendingPurchaseOrders = await _context.PurchaseOrders.CountAsync(p => p.Status == PurchaseOrderStatus.Pending || p.Status == PurchaseOrderStatus.Approved),
                    OpenInvoices = await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Sent || i.Status == InvoiceStatus.PartiallyPaid || i.Status == InvoiceStatus.Overdue),
                    TotalOutstandingAmount = await _context.Invoices
                                                    .Where(i => i.Status == InvoiceStatus.Sent || i.Status == InvoiceStatus.PartiallyPaid || i.Status == InvoiceStatus.Overdue)
                                                    .SumAsync(i => i.TotalAmount - i.AmountPaid)
                };

                // --- Chart Data ---
                // Weekly Sales Chart
                var weeklySales = await _context.Sales
                                                .Where(s => s.SaleDate >= startOfWeek && s.SaleDate < endOfWeek)
                                                .GroupBy(s => s.SaleDate.Date)
                                                .Select(g => new { Date = g.Key, Total = g.Sum(s => s.TotalAmount) })
                                                .OrderBy(x => x.Date)
                                                .ToListAsync();

                var weeklySalesChart = new ChartDataDto<decimal>();
                for (int i = 0; i < 7; i++)
                {
                    var date = startOfWeek.AddDays(i);
                    weeklySalesChart.Labels.Add(date.ToString("ddd"));
                    var sale = weeklySales.FirstOrDefault(s => s.Date == date);
                    weeklySalesChart.Data.Add(sale?.Total ?? 0);
                }

                // Appointment Status Chart for today
                var appointmentStatusGroups = await _context.Appointments
                                                        .Where(a => a.AppointmentDateTime.Date == today)
                                                        .GroupBy(a => a.Status)
                                                        .Select(g => new { Status = g.Key, Count = g.Count() })
                                                        .ToListAsync();

                var appointmentStatusChart = new ChartDataDto<int>
                {
                    Labels = appointmentStatusGroups.Select(g => g.Status.ToString()).ToList(),
                    Data = appointmentStatusGroups.Select(g => g.Count).ToList()
                };


                return new DashboardDto
                {
                    Stats = statsDto,
                    WeeklySalesChart = weeklySalesChart,
                    AppointmentStatusChart = appointmentStatusChart
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dashboard data.");
                // Return an empty DTO on failure
                return new DashboardDto();
            }
        }
    }
}
