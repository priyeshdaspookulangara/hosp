using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public interface IOTScheduleService
    {
        Task<IEnumerable<OTScheduleDto>> GetAllOTSchedulesAsync();
        Task<OTScheduleDto> GetOTScheduleByIdAsync(Guid id);
        Task<OTScheduleDto> CreateOTScheduleAsync(CreateOTScheduleDto createOTScheduleDto);
        Task<bool> UpdateOTScheduleAsync(UpdateOTScheduleDto updateOTScheduleDto);
        Task<bool> DeleteOTScheduleAsync(Guid id);
    }
}
