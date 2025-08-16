using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Emergency;
using UniCareERP.Domain.Entities.Emergency;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Emergency
{
    public class EmergencyCaseService : IEmergencyCaseService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<EmergencyCaseService> _logger;

        public EmergencyCaseService(UniCareDbContext context, ILogger<EmergencyCaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EmergencyCaseDto> CreateEmergencyCaseAsync(CreateEmergencyCaseDto createEmergencyCaseDto)
        {
            var emergencyCase = new EmergencyCase
            {
                PatientId = createEmergencyCaseDto.PatientId,
                CaseDescription = createEmergencyCaseDto.CaseDescription
            };

            _context.EmergencyCases.Add(emergencyCase);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Emergency case created with ID: {emergencyCase.Id}");

            return await GetEmergencyCaseByIdAsync(emergencyCase.Id);
        }

        public async Task<bool> DeleteEmergencyCaseAsync(Guid id)
        {
            var emergencyCase = await _context.EmergencyCases.FindAsync(id);
            if (emergencyCase == null)
            {
                return false;
            }

            _context.EmergencyCases.Remove(emergencyCase);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Emergency case with ID: {id} deleted.");

            return true;
        }

        public async Task<IEnumerable<EmergencyCaseDto>> GetAllEmergencyCasesAsync()
        {
            return await _context.EmergencyCases
                .Include(ec => ec.Patient)
                .Select(ec => new EmergencyCaseDto
                {
                    Id = ec.Id,
                    PatientId = ec.PatientId,
                    PatientName = ec.Patient.FirstName + " " + ec.Patient.LastName,
                    CaseDescription = ec.CaseDescription,
                    ReportedAt = ec.ReportedAt,
                    Status = ec.Status
                })
                .ToListAsync();
        }

        public async Task<EmergencyCaseDto> GetEmergencyCaseByIdAsync(Guid id)
        {
            var emergencyCase = await _context.EmergencyCases
                .Include(ec => ec.Patient)
                .FirstOrDefaultAsync(ec => ec.Id == id);

            if (emergencyCase == null)
            {
                return null;
            }

            return new EmergencyCaseDto
            {
                Id = emergencyCase.Id,
                PatientId = emergencyCase.PatientId,
                PatientName = emergencyCase.Patient.FirstName + " " + emergencyCase.Patient.LastName,
                CaseDescription = emergencyCase.CaseDescription,
                ReportedAt = emergencyCase.ReportedAt,
                Status = emergencyCase.Status
            };
        }

        public async Task<EmergencyCaseDto> UpdateEmergencyCaseAsync(UpdateEmergencyCaseDto updateEmergencyCaseDto)
        {
            var emergencyCase = await _context.EmergencyCases.FindAsync(updateEmergencyCaseDto.Id);

            if (emergencyCase == null)
            {
                return null;
            }

            emergencyCase.CaseDescription = updateEmergencyCaseDto.CaseDescription;
            emergencyCase.Status = updateEmergencyCaseDto.Status;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Emergency case with ID: {emergencyCase.Id} updated.");

            return await GetEmergencyCaseByIdAsync(emergencyCase.Id);
        }
    }
}
