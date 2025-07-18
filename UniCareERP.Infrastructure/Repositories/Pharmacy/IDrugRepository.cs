using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Pharmacy;

namespace UniCareERP.Infrastructure.Repositories.Pharmacy
{
    public interface IDrugRepository
    {
        Task<IEnumerable<Drug>> GetAllAsync();
        Task<Drug> GetByIdAsync(int id);
        Task<Drug> AddAsync(Drug drug);
        Task UpdateAsync(Drug drug);
        Task DeleteAsync(int id);
    }
}
