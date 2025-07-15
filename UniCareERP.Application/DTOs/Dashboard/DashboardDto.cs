using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Dashboard
{
    public class ChartDataDto<T>
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<T> Data { get; set; } = new List<T>();
    }

    public class DashboardStatsDto
    {
        // Key KPI Cards
        public int TotalActivePatients { get; set; }
        public int AppointmentsToday { get; set; }
        public decimal TotalSalesToday { get; set; }
        public int LowStockItems { get; set; }

        // Other interesting stats
        public int PendingPurchaseOrders { get; set; }
        public int OpenInvoices { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
    }

    public class DashboardDto
    {
        public DashboardStatsDto Stats { get; set; } = new DashboardStatsDto();

        // Data for charts
        public ChartDataDto<decimal>? WeeklySalesChart { get; set; }
        public ChartDataDto<int>? AppointmentStatusChart { get; set; } // e.g., Pie chart of Scheduled/Completed/Cancelled today
    }
}
