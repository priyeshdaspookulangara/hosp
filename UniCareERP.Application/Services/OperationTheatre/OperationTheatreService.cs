using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs;
using UniCareERP.Application.DTOs.OperationTheatre;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Domain.Entities.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public class OperationTheatreService : IOperationTheatreService
    {
        private readonly UniCareDbContext _context;

        public OperationTheatreService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperationTheatreDto>> GetAllOperationTheatresAsync()
        {
            return await _context.OperationTheatres
                .Select(ot => new OperationTheatreDto
                {
                    Id = ot.Id,
                    Name = ot.Name,
                    RoomNumber = ot.RoomNumber,
                    IsAvailable = ot.IsAvailable,
                    Location = ot.Location,
                    Equipment = ot.Equipment
                })
                .ToListAsync();
        }

        public async Task<OperationTheatreDto> GetOperationTheatreByIdAsync(Guid id)
        {
            var ot = await _context.OperationTheatres.FindAsync(id);
            if (ot == null) return null;

            return new OperationTheatreDto
            {
                Id = ot.Id,
                Name = ot.Name,
                RoomNumber = ot.RoomNumber,
                IsAvailable = ot.IsAvailable,
                Location = ot.Location,
                Equipment = ot.Equipment
            };
        }

        public async Task<OperationTheatreDto> CreateOperationTheatreAsync(CreateOperationTheatreDto createOperationTheatreDto)
        {
            var ot = new Domain.Entities.OperationTheatre.OperationTheatre
            {
                Name = createOperationTheatreDto.Name,
                RoomNumber = createOperationTheatreDto.RoomNumber,
                IsAvailable = createOperationTheatreDto.IsAvailable,
                Location = createOperationTheatreDto.Location,
                Equipment = createOperationTheatreDto.Equipment
            };

            _context.OperationTheatres.Add(ot);
            await _context.SaveChangesAsync();

            return new OperationTheatreDto
            {
                Id = ot.Id,
                Name = ot.Name,
                RoomNumber = ot.RoomNumber,
                IsAvailable = ot.IsAvailable,
                Location = ot.Location,
                Equipment = ot.Equipment
            };
        }

        public async Task<bool> UpdateOperationTheatreAsync(UpdateOperationTheatreDto updateOperationTheatreDto)
        {
            var ot = await _context.OperationTheatres.FindAsync(updateOperationTheatreDto.Id);
            if (ot == null) return false;

            ot.Name = updateOperationTheatreDto.Name;
            ot.RoomNumber = updateOperationTheatreDto.RoomNumber;
            ot.IsAvailable = updateOperationTheatreDto.IsAvailable;
            ot.Location = updateOperationTheatreDto.Location;
            ot.Equipment = updateOperationTheatreDto.Equipment;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOperationTheatreAsync(Guid id)
        {
            var ot = await _context.OperationTheatres.FindAsync(id);
            if (ot == null) return false;

            _context.OperationTheatres.Remove(ot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SurgicalProcedureDto>> GetAllSurgicalProceduresAsync()
        {
            return await _context.SurgicalProcedures
                .Select(sp => new SurgicalProcedureDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Description = sp.Description,
                    RequiredEquipment = sp.RequiredEquipment,
                    DurationMinutes = sp.DurationMinutes
                })
                .ToListAsync();
        }

        public async Task<SurgicalProcedureDto> GetSurgicalProcedureByIdAsync(Guid id)
        {
            var sp = await _context.SurgicalProcedures.FindAsync(id);
            if (sp == null) return null;

            return new SurgicalProcedureDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                RequiredEquipment = sp.RequiredEquipment,
                DurationMinutes = sp.DurationMinutes
            };
        }

        public async Task<SurgicalProcedureDto> CreateSurgicalProcedureAsync(CreateSurgicalProcedureDto createSurgicalProcedureDto)
        {
            var sp = new SurgicalProcedure
            {
                Name = createSurgicalProcedureDto.Name,
                Description = createSurgicalProcedureDto.Description,
                RequiredEquipment = createSurgicalProcedureDto.RequiredEquipment,
                DurationMinutes = createSurgicalProcedureDto.DurationMinutes
            };

            _context.SurgicalProcedures.Add(sp);
            await _context.SaveChangesAsync();

            return new SurgicalProcedureDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                RequiredEquipment = sp.RequiredEquipment,
                DurationMinutes = sp.DurationMinutes
            };
        }

        public async Task<bool> UpdateSurgicalProcedureAsync(UpdateSurgicalProcedureDto updateSurgicalProcedureDto)
        {
            var sp = await _context.SurgicalProcedures.FindAsync(updateSurgicalProcedureDto.Id);
            if (sp == null) return false;

            sp.Name = updateSurgicalProcedureDto.Name;
            sp.Description = updateSurgicalProcedureDto.Description;
            sp.RequiredEquipment = updateSurgicalProcedureDto.RequiredEquipment;
            sp.DurationMinutes = updateSurgicalProcedureDto.DurationMinutes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSurgicalProcedureAsync(Guid id)
        {
            var sp = await _context.SurgicalProcedures.FindAsync(id);
            if (sp == null) return false;

            _context.SurgicalProcedures.Remove(sp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SurgicalTeamDto>> GetAllSurgicalTeamsAsync()
        {
            return await _context.SurgicalTeams
                .Include(st => st.Members)
                .Select(st => new SurgicalTeamDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Members = st.Members.Select(m => new UserDto
                    {
                        Id = m.Id,
                        UserName = m.UserName,
                        Email = m.Email
                    })
                })
                .ToListAsync();
        }

        public async Task<SurgicalTeamDto> GetSurgicalTeamByIdAsync(Guid id)
        {
            var st = await _context.SurgicalTeams
                .Include(st => st.Members)
                .FirstOrDefaultAsync(st => st.Id == id);
            if (st == null) return null;

            return new SurgicalTeamDto
            {
                Id = st.Id,
                Name = st.Name,
                Members = st.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    UserName = m.UserName,
                    Email = m.Email
                })
            };
        }

        public async Task<SurgicalTeamDto> CreateSurgicalTeamAsync(CreateSurgicalTeamDto createSurgicalTeamDto)
        {
            var members = await _context.Users
                .Where(u => createSurgicalTeamDto.MemberIds.Contains(u.Id))
                .ToListAsync();

            var st = new SurgicalTeam
            {
                Name = createSurgicalTeamDto.Name,
                Members = members
            };

            _context.SurgicalTeams.Add(st);
            await _context.SaveChangesAsync();

            return new SurgicalTeamDto
            {
                Id = st.Id,
                Name = st.Name,
                Members = st.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    UserName = m.UserName,
                    Email = m.Email
                })
            };
        }

        public async Task<bool> UpdateSurgicalTeamAsync(UpdateSurgicalTeamDto updateSurgicalTeamDto)
        {
            var st = await _context.SurgicalTeams
                .Include(st => st.Members)
                .FirstOrDefaultAsync(st => st.Id == updateSurgicalTeamDto.Id);
            if (st == null) return false;

            var members = await _context.Users
                .Where(u => updateSurgicalTeamDto.MemberIds.Contains(u.Id))
                .ToListAsync();

            st.Name = updateSurgicalTeamDto.Name;
            st.Members = members;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSurgicalTeamAsync(Guid id)
        {
            var st = await _context.SurgicalTeams.FindAsync(id);
            if (st == null) return false;

            _context.SurgicalTeams.Remove(st);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
