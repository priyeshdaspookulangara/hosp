using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Radiology
{
    public class RadiologyReportService : IRadiologyReportService
    {
        private readonly UniCareDbContext _context;

        public RadiologyReportService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RadiologyReport>> GetAllRadiologyReportsAsync()
        {
            return await _context.RadiologyReports.ToListAsync();
        }

        public async Task<RadiologyReport> GetRadiologyReportByIdAsync(Guid id)
        {
            return await _context.RadiologyReports.FindAsync(id);
        }

        public async Task AddRadiologyReportAsync(RadiologyReport radiologyReport)
        {
            _context.RadiologyReports.Add(radiologyReport);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRadiologyReportAsync(RadiologyReport radiologyReport)
        {
            _context.Entry(radiologyReport).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRadiologyReportAsync(Guid id)
        {
            var radiologyReport = await _context.RadiologyReports.FindAsync(id);
            _context.RadiologyReports.Remove(radiologyReport);
            await _context.SaveChangesAsync();
        }
    }
}
