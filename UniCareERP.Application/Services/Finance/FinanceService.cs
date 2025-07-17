using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.Services.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly IGeneralLedgerService _generalLedgerService;

        public FinanceService(
            IInvoiceService invoiceService,
            IPaymentService paymentService,
            IGeneralLedgerService generalLedgerService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _generalLedgerService = generalLedgerService;
        }

        public async Task<FinanceDashboardDto> GetFinanceDashboardDataAsync()
        {
            var allInvoices = await _invoiceService.GetAllInvoicesAsync();
            var recentInvoices = allInvoices.OrderByDescending(i => i.InvoiceDate).Take(10);

            var totalRevenue = allInvoices
                .Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PartiallyPaid)
                .Sum(i => i.AmountPaid);

            var totalAccountsReceivable = allInvoices
                .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Void)
                .Sum(i => i.AmountDue);

            // Placeholder for expenses and upcoming bills
            var dashboardData = new FinanceDashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalAccountsReceivable = totalAccountsReceivable,
                RecentInvoices = recentInvoices,
                TotalExpenses = 0, // To be implemented
                TotalAccountsPayable = 0, // To be implemented
                RecentPayments = new List<object>(), // To be implemented
                UpcomingBills = new List<object>() // To be implemented
            };

            return dashboardData;
        }
    }
}
