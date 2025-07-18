using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Pharmacy;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Infrastructure.Repositories.Pharmacy
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly UniCareDbContext _context;

        public PrescriptionRepository(UniCareDbContext context)
        {
            _context = context;
        }

        public Task<Prescription> AddAsync(Prescription prescription)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Prescription>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Prescription> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Prescription prescription)
        {
            throw new System.NotImplementedException();
        }
    }
}
