using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Pharmacy;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Infrastructure.Repositories.Pharmacy
{
    public class DrugRepository : IDrugRepository
    {
        private readonly UniCareDbContext _context;

        public DrugRepository(UniCareDbContext context)
        {
            _context = context;
        }

        public Task<Drug> AddAsync(Drug drug)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Drug>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Drug> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Drug drug)
        {
            throw new System.NotImplementedException();
        }
    }
}
