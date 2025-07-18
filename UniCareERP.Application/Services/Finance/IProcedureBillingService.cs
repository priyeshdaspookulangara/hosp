using System;
using System.Threading.Tasks;

namespace UniCareERP.Application.Services.Finance
{
    public interface IProcedureBillingService
    {
        Task GenerateChargesAsync(Guid procedureId);
    }
}
