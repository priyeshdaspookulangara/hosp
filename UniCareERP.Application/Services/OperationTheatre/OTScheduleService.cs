using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.OperationTheatre;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Domain.Entities.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public class OTScheduleService : IOTScheduleService
    {
        private readonly UniCareDbContext _context;

        public OTScheduleService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OTScheduleDto>> GetAllOTSchedulesAsync()
        {
            return await _context.OTSchedules
                .Include(s => s.OperationTheatre)
                .Include(s => s.SurgicalProcedure)
                .Include(s => s.Patient)
                .Include(s => s.SurgicalTeam)
                .Select(s => new OTScheduleDto
                {
                    Id = s.Id,
                    OperationTheatreId = s.OperationTheatreId,
                    OperationTheatreName = s.OperationTheatre.Name,
                    SurgicalProcedureId = s.SurgicalProcedureId,
                    SurgicalProcedureName = s.SurgicalProcedure.Name,
                    PatientId = s.PatientId,
                    PatientName = s.Patient.FirstName + " " + s.Patient.LastName,
                    SurgicalTeamId = s.SurgicalTeamId,
                    SurgicalTeamName = s.SurgicalTeam.Name,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes
                })
                .ToListAsync();
        }

        public async Task<OTScheduleDto> GetOTScheduleByIdAsync(Guid id)
        {
            var s = await _context.OTSchedules
                .Include(s => s.OperationTheatre)
                .Include(s => s.SurgicalProcedure)
                .Include(s => s.Patient)
                .Include(s => s.SurgicalTeam)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (s == null) return null;

            return new OTScheduleDto
            {
                Id = s.Id,
                OperationTheatreId = s.OperationTheatreId,
                OperationTheatreName = s.OperationTheatre.Name,
                SurgicalProcedureId = s.SurgicalProcedureId,
                SurgicalProcedureName = s.SurgicalProcedure.Name,
                PatientId = s.PatientId,
                PatientName = s.Patient.FirstName + " " + s.Patient.LastName,
                SurgicalTeamId = s.SurgicalTeamId,
                SurgicalTeamName = s.SurgicalTeam.Name,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Notes = s.Notes
            };
        }

        public async Task<OTScheduleDto> CreateOTScheduleAsync(CreateOTScheduleDto createOTScheduleDto)
        {
            var schedule = new OTSchedule
            {
                OperationTheatreId = createOTScheduleDto.OperationTheatreId,
                SurgicalProcedureId = createOTScheduleDto.SurgicalProcedureId,
                PatientId = createOTScheduleDto.PatientId,
                SurgicalTeamId = createOTScheduleDto.SurgicalTeamId,
                StartTime = createOTScheduleDto.StartTime,
                EndTime = createOTScheduleDto.EndTime,
                Notes = createOTScheduleDto.Notes
            };

            _context.OTSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            var newSchedule = await GetOTScheduleByIdAsync(schedule.Id);
            return newSchedule;
        }

        public async Task<bool> UpdateOTScheduleAsync(UpdateOTScheduleDto updateOTScheduleDto)
        {
            var schedule = await _context.OTSchedules.FindAsync(updateOTScheduleDto.Id);
            if (schedule == null) return false;

            schedule.OperationTheatreId = updateOTScheduleDto.OperationTheatreId;
            schedule.SurgicalProcedureId = updateOTScheduleDto.SurgicalProcedureId;
            schedule.PatientId = updateOTScheduleDto.PatientId;
            schedule.SurgicalTeamId = updateOTScheduleDto.SurgicalTeamId;
            schedule.StartTime = updateOTScheduleDto.StartTime;
            schedule.EndTime = updateOTScheduleDto.EndTime;
            schedule.Notes = updateOTScheduleDto.Notes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOTScheduleAsync(Guid id)
        {
            var schedule = await _context.OTSchedules.FindAsync(id);
            if (schedule == null) return false;

            _context.OTSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
