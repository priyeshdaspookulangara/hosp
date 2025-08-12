using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Schedules;

namespace UniCareERP.Infrastructure.Repositories.Schedules
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule> GetByIdAsync(Guid id);
        Task AddAsync(Schedule schedule);
        void Update(Schedule schedule);
        void Remove(Schedule schedule);
    }
}
