using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Domain.Entities.Lab;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace UniCareERP.Application.Services.Lab
{
    public class LabService : ILabService
    {
        private readonly UniCareDbContext _context;

        public LabService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<LabTestDto> CreateLabTestAsync(LabTestDto labTestDto)
        {
            var labTest = new LabTest
            {
                Name = labTestDto.Name,
                Description = labTestDto.Description,
                Price = labTestDto.Price,
                NormalRange = labTestDto.NormalRange
            };

            _context.LabTests.Add(labTest);
            await _context.SaveChangesAsync();

            labTestDto.Id = labTest.Id;
            return labTestDto;
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

        public async Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync()
        {
            return await _context.LabTests
                .Select(lt => new LabTestDto
                {
                    Id = lt.Id,
                    Name = lt.Name,
                    Description = lt.Description,
                    Price = lt.Price,
                    NormalRange = lt.NormalRange
                })
                .ToListAsync();
        }

        public async Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest == null) return null;

            return new LabTestDto
            {
                Id = labTest.Id,
                Name = labTest.Name,
                Description = labTest.Description,
                Price = labTest.Price,
                NormalRange = labTest.NormalRange
            };
        }

        public async Task UpdateLabTestAsync(Guid id, LabTestDto labTestDto)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest != null)
            {
                labTest.Name = labTestDto.Name;
                labTest.Description = labTestDto.Description;
                labTest.Price = labTestDto.Price;
                labTest.NormalRange = labTestDto.NormalRange;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<LabOrderDto> CreateLabOrderAsync(LabOrderDto labOrderDto)
        {
            var labOrder = new LabOrder
            {
                PatientId = labOrderDto.PatientId,
                LabTestId = labOrderDto.LabTestId,
                OrderDate = labOrderDto.OrderDate,
                Status = labOrderDto.Status,
                Result = labOrderDto.Result
            };

            _context.LabOrders.Add(labOrder);
            await _context.SaveChangesAsync();

            labOrderDto.Id = labOrder.Id;
            return labOrderDto;
        }

        public async Task DeleteLabOrderAsync(Guid id)
        {
            var labOrder = await _context.LabOrders.FindAsync(id);
            if (labOrder != null)
            {
                _context.LabOrders.Remove(labOrder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<LabOrderDto>> GetAllLabOrdersAsync()
        {
            return await _context.LabOrders
                .Select(lo => new LabOrderDto
                {
                    Id = lo.Id,
                    PatientId = lo.PatientId,
                    LabTestId = lo.LabTestId,
                    OrderDate = lo.OrderDate,
                    Status = lo.Status,
                    Result = lo.Result
                })
                .ToListAsync();
        }

        public async Task<LabOrderDto> GetLabOrderByIdAsync(Guid id)
        {
            var labOrder = await _context.LabOrders.FindAsync(id);
            if (labOrder == null) return null;

            return new LabOrderDto
            {
                Id = labOrder.Id,
                PatientId = labOrder.PatientId,
                LabTestId = labOrder.LabTestId,
                OrderDate = labOrder.OrderDate,
                Status = labOrder.Status,
                Result = labOrder.Result
            };
        }

        public async Task<IEnumerable<LabOrderDto>> GetLabOrdersByPatientIdAsync(Guid patientId)
        {
            return await _context.LabOrders
                .Where(lo => lo.PatientId == patientId)
                .Include(lo => lo.LabTest)
                .Select(lo => new LabOrderDto
                {
                    Id = lo.Id,
                    PatientId = lo.PatientId,
                    LabTestId = lo.LabTestId,
                    LabTestName = lo.LabTest.Name,
                    OrderDate = lo.OrderDate,
                    Status = lo.Status,
                    Result = lo.Result
                })
                .ToListAsync();
        }

        public async Task UpdateLabOrderAsync(Guid id, LabOrderDto labOrderDto)
        {
            var labOrder = await _context.LabOrders.FindAsync(id);
            if (labOrder != null)
            {
                labOrder.Status = labOrderDto.Status;
                labOrder.Result = labOrderDto.Result;

                await _context.SaveChangesAsync();
            }
        }
    }
}
