using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public class SurgeryService : ISurgeryService
    {
        public Task<SurgeryDto> CreateSurgeryAsync(SurgeryDto surgeryDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSurgeryAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SurgeryDto>> GetAllSurgeriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SurgeryDto> GetSurgeryByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSurgeryAsync(Guid id, SurgeryDto surgeryDto)
        {
            throw new NotImplementedException();
        }
    }
}
