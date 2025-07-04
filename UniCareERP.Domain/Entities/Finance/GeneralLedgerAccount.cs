using System;

namespace UniCareERP.Domain.Entities.Finance
{
    public class GeneralLedgerAccount
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty; // e.g., 10100 (Cash), 40100 (Revenue)
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense (Enum later)
        public decimal Balance { get; set; } // Current balance
        public bool IsActive { get; set; } = true;

        // public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();

        public GeneralLedgerAccount()
        {
            Id = Guid.NewGuid();
        }
    }
}
