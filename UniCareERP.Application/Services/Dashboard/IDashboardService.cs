using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Dashboard;

namespace UniCareERP.Application.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
