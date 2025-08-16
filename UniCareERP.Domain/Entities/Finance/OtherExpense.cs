using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Finance
{
    public class OtherExpense
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }

        public Guid GeneralLedgerAccountId { get; set; }
        public virtual GeneralLedgerAccount? GeneralLedgerAccount { get; set; }

        public OtherExpense()
        {
            Id = Guid.NewGuid();
            ExpenseDate = DateTime.UtcNow;
        }
    }
}
