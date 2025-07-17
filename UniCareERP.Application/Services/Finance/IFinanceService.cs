using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IFinanceService
    {
        Task<FinanceDashboardDto> GetFinanceDashboardDataAsync();
    }
}
