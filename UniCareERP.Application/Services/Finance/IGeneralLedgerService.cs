using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IGeneralLedgerService
    {
        Task PostTransactionAsync(GeneralLedgerAccount creditAccount, GeneralLedgerAccount debitAccount, decimal amount, string description);
    }
}
