using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Radiology
{
    public class ImagingServiceService : IImagingService
    {
        private readonly UniCareDbContext _context;

        public ImagingServiceService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImagingService>> GetAllImagingServicesAsync()
        {
            return await _context.ImagingServices.ToListAsync();
        }

        public async Task<ImagingService> GetImagingServiceByIdAsync(Guid id)
        {
            return await _context.ImagingServices.FindAsync(id);
        }

        public async Task AddImagingServiceAsync(ImagingService imagingService)
        {
            _context.ImagingServices.Add(imagingService);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImagingServiceAsync(ImagingService imagingService)
        {
            _context.Entry(imagingService).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImagingServiceAsync(Guid id)
        {
            var imagingService = await _context.ImagingServices.FindAsync(id);
            _context.ImagingServices.Remove(imagingService);
            await _context.SaveChangesAsync();
        }
    }
}
