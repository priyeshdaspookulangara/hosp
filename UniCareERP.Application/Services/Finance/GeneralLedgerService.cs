using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class GeneralLedgerService : IGeneralLedgerService
    {
        private readonly UniCareDbContext _context;

        public GeneralLedgerService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task PostTransactionAsync(GeneralLedgerAccount creditAccount, GeneralLedgerAccount debitAccount, decimal amount, string description)
        {
            creditAccount.Balance -= amount;
            debitAccount.Balance += amount;

            _context.GeneralLedgerAccounts.Update(creditAccount);
            _context.GeneralLedgerAccounts.Update(debitAccount);

            await _context.SaveChangesAsync();
        }
    }
}
