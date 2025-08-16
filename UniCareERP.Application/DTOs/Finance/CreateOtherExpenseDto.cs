using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Finance
{
    public class CreateOtherExpenseDto
    {
        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        [Required]
        public Guid GeneralLedgerAccountId { get; set; }

        [Required]
        public Guid CreditAccountId { get; set; }
    }
}
