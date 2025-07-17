using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Radiology
{
    public class WorklistItemService : IWorklistItemService
    {
        private readonly UniCareDbContext _context;

        public WorklistItemService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorklistItem>> GetAllWorklistItemsAsync()
        {
            return await _context.WorklistItems.ToListAsync();
        }

        public async Task<WorklistItem> GetWorklistItemByIdAsync(Guid id)
        {
            return await _context.WorklistItems.FindAsync(id);
        }

        public async Task AddWorklistItemAsync(WorklistItem worklistItem)
        {
            _context.WorklistItems.Add(worklistItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWorklistItemAsync(WorklistItem worklistItem)
        {
            _context.Entry(worklistItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorklistItemAsync(Guid id)
        {
            var worklistItem = await _context.WorklistItems.FindAsync(id);
            _context.WorklistItems.Remove(worklistItem);
            await _context.SaveChangesAsync();
        }
    }
}
