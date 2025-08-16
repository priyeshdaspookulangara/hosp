using System;

namespace UniCareERP.Application.DTOs.Finance
{
    public class OtherExpenseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public Guid GeneralLedgerAccountId { get; set; }
        public string GeneralLedgerAccountName { get; set; } = string.Empty;
    }
}
