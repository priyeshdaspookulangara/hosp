using System;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IGeneralLedgerService
    {
        Task RecordTransactionAsync(Guid accountId, DateTime date, decimal amount, string description);
    }
}
