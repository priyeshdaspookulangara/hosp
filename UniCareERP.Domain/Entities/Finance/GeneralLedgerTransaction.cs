using System;

namespace UniCareERP.Domain.Entities.Finance
{
    public class GeneralLedgerTransaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public GeneralLedgerAccount? Account { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
