using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Schedules;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Infrastructure.Repositories.Schedules
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly UniCareDbContext _context;

        public ScheduleRepository(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules.Include(s => s.Doctor).ToListAsync();
        }

        public async Task<Schedule> GetByIdAsync(Guid id)
        {
            return await _context.Schedules.Include(s => s.Doctor).FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Remove(Schedule schedule)
        {
            _context.Schedules.Remove(schedule);
            _context.SaveChanges();
        }

        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            _context.SaveChanges();
        }
    }
}
