using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.Lab;
using UniCareERP.Domain.Entities.Lab;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Lab
{
    public class LabTestService : ILabTestService
    {
        private readonly UniCareDbContext _context;

        public LabTestService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            return labTest == null ? null : MapToDto(labTest);
        }

        public async Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync()
        {
            return await _context.LabTests
                .Select(lt => MapToDto(lt))
                .ToListAsync();
        }

        public async Task<LabTestDto> CreateLabTestAsync(LabTestDto labTestDto)
        {
            var labTest = new LabTest
            {
                Name = labTestDto.Name,
                Description = labTestDto.Description,
                Category = labTestDto.Category,
                Price = labTestDto.Price
            };

            _context.LabTests.Add(labTest);
            await _context.SaveChangesAsync();

            return MapToDto(labTest);
        }

        public async Task<LabTestDto> UpdateLabTestAsync(LabTestDto labTestDto)
        {
            var labTest = await _context.LabTests.FindAsync(labTestDto.Id);
            if (labTest == null) return null;

            labTest.Name = labTestDto.Name;
            labTest.Description = labTestDto.Description;
            labTest.Category = labTestDto.Category;
            labTest.Price = labTestDto.Price;

            await _context.SaveChangesAsync();

            return MapToDto(labTest);
        }

        public async Task DeleteLabTestAsync(Guid id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest != null)
            {
                _context.LabTests.Remove(labTest);
                await _context.SaveChangesAsync();
            }
        }

        private static LabTestDto MapToDto(LabTest labTest)
        {
            return new LabTestDto
            {
                Id = labTest.Id,
                Name = labTest.Name,
                Description = labTest.Description,
                Category = labTest.Category,
                Price = labTest.Price
            };
        }
    }
}
