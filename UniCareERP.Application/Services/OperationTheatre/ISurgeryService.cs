using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public interface ISurgeryService
    {
        Task<IEnumerable<SurgeryDto>> GetAllSurgeriesAsync();
        Task<SurgeryDto> GetSurgeryByIdAsync(Guid id);
        Task<SurgeryDto> CreateSurgeryAsync(SurgeryDto surgeryDto);
        Task UpdateSurgeryAsync(Guid id, SurgeryDto surgeryDto);
        Task DeleteSurgeryAsync(Guid id);
    }
}
