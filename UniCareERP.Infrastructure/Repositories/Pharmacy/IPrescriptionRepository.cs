using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Pharmacy;

namespace UniCareERP.Infrastructure.Repositories.Pharmacy
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetAllAsync();
        Task<Prescription> GetByIdAsync(int id);
        Task<Prescription> AddAsync(Prescription prescription);
        Task UpdateAsync(Prescription prescription);
        Task DeleteAsync(int id);
    }
}
