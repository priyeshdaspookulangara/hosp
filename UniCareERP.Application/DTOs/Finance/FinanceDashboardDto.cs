using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Finance
{
    public class FinanceDashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome => TotalRevenue - TotalExpenses;
        public decimal TotalAccountsReceivable { get; set; }
        public decimal TotalAccountsPayable { get; set; }
        public IEnumerable<InvoiceDto>? RecentInvoices { get; set; }
        public IEnumerable<object>? RecentPayments { get; set; } // Replace with PaymentDto later
        public IEnumerable<object>? UpcomingBills { get; set; } // Replace with BillDto later
    }
}
