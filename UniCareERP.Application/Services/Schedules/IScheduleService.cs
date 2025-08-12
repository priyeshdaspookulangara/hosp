using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Schedules;

namespace UniCareERP.Application.Services.Schedules
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetSchedulesAsync();
        Task<ScheduleDto> GetScheduleByIdAsync(Guid id);
        Task<ScheduleDto> CreateScheduleAsync(ScheduleDto scheduleDto);
        Task UpdateScheduleAsync(Guid id, ScheduleDto scheduleDto);
        Task DeleteScheduleAsync(Guid id);
    }
}
