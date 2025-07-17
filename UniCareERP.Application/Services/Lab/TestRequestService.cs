using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.Lab;
using UniCareERP.Domain.Entities.Lab;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.DTOs.Finance;

namespace UniCareERP.Application.Services.Lab
{
    public class TestRequestService : ITestRequestService
    {
        private readonly UniCareDbContext _context;
        private readonly IInvoiceService _invoiceService;

        public TestRequestService(UniCareDbContext context, IInvoiceService invoiceService)
        {
            _context = context;
            _invoiceService = invoiceService;
        }

        public async Task<TestRequestDto> GetTestRequestByIdAsync(Guid id)
        {
            var testRequest = await _context.TestRequests
                .Include(tr => tr.Patient)
                .Include(tr => tr.LabTests)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            return testRequest == null ? null : MapToDto(testRequest);
        }

        public async Task<IEnumerable<TestRequestDto>> GetAllTestRequestsAsync()
        {
            return await _context.TestRequests
                .Include(tr => tr.Patient)
                .Include(tr => tr.LabTests)
                .Select(tr => MapToDto(tr))
                .ToListAsync();
        }

        public async Task<TestRequestDto> CreateTestRequestAsync(TestRequestDto testRequestDto)
        {
            var testRequest = new TestRequest
            {
                PatientId = testRequestDto.PatientId,
                DoctorId = testRequestDto.DoctorId,
                RequestDate = testRequestDto.RequestDate,
                LabTests = await _context.LabTests.Where(lt => testRequestDto.LabTests.Select(ltdto => ltdto.Id).Contains(lt.Id)).ToListAsync()
            };

            _context.TestRequests.Add(testRequest);
            await _context.SaveChangesAsync();

            var invoiceDto = new CreateInvoiceDto
            {
                PatientId = testRequest.PatientId,
                InvoiceDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                InvoiceItems = testRequest.LabTests.Select(lt => new CreateInvoiceItemDto
                {
                    Description = lt.Name,
                    Quantity = 1,
                    UnitPrice = lt.Price
                }).ToList()
            };

            await _invoiceService.CreateInvoiceAsync(invoiceDto);

            return MapToDto(testRequest);
        }

        public async Task<TestRequestDto> UpdateTestRequestAsync(TestRequestDto testRequestDto)
        {
            var testRequest = await _context.TestRequests
                .Include(tr => tr.LabTests)
                .FirstOrDefaultAsync(tr => tr.Id == testRequestDto.Id);

            if (testRequest == null) return null;

            testRequest.PatientId = testRequestDto.PatientId;
            testRequest.DoctorId = testRequestDto.DoctorId;
            testRequest.RequestDate = testRequestDto.RequestDate;
            testRequest.LabTests = await _context.LabTests.Where(lt => testRequestDto.LabTests.Select(ltdto => ltdto.Id).Contains(lt.Id)).ToListAsync();

            await _context.SaveChangesAsync();

            return MapToDto(testRequest);
        }

        public async Task DeleteTestRequestAsync(Guid id)
        {
            var testRequest = await _context.TestRequests.FindAsync(id);
            if (testRequest != null)
            {
                _context.TestRequests.Remove(testRequest);
                await _context.SaveChangesAsync();
            }
        }

        private static TestRequestDto MapToDto(TestRequest testRequest)
        {
            return new TestRequestDto
            {
                Id = testRequest.Id,
                PatientId = testRequest.PatientId,
                PatientName = testRequest.Patient?.FirstName + " " + testRequest.Patient?.LastName,
                DoctorId = testRequest.DoctorId,
                RequestDate = testRequest.RequestDate,
                LabTests = testRequest.LabTests.Select(lt => new LabTestDto
                {
                    Id = lt.Id,
                    Name = lt.Name,
                    Description = lt.Description,
                    Category = lt.Category,
                    Price = lt.Price
                }).ToList()
            };
        }
    }
}
