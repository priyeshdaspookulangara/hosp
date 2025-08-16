using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Emergency;

namespace UniCareERP.Application.Services.Emergency
{
    public interface IEmergencyCaseService
    {
        Task<EmergencyCaseDto> CreateEmergencyCaseAsync(CreateEmergencyCaseDto createEmergencyCaseDto);
        Task<EmergencyCaseDto> GetEmergencyCaseByIdAsync(Guid id);
        Task<IEnumerable<EmergencyCaseDto>> GetAllEmergencyCasesAsync();
        Task<EmergencyCaseDto> UpdateEmergencyCaseAsync(UpdateEmergencyCaseDto updateEmergencyCaseDto);
        Task<bool> DeleteEmergencyCaseAsync(Guid id);
    }
}
