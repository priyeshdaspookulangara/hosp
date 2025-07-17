using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Radiology
{
    public class ImagingTestService : IImagingTestService
    {
        private readonly UniCareDbContext _context;

        public ImagingTestService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImagingTest>> GetAllImagingTestsAsync()
        {
            return await _context.ImagingTests.ToListAsync();
        }

        public async Task<ImagingTest> GetImagingTestByIdAsync(Guid id)
        {
            return await _context.ImagingTests.FindAsync(id);
        }

        public async Task AddImagingTestAsync(ImagingTest imagingTest)
        {
            _context.ImagingTests.Add(imagingTest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImagingTestAsync(ImagingTest imagingTest)
        {
            _context.Entry(imagingTest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImagingTestAsync(Guid id)
        {
            var imagingTest = await _context.ImagingTests.FindAsync(id);
            _context.ImagingTests.Remove(imagingTest);
            await _context.SaveChangesAsync();
        }
    }
}
