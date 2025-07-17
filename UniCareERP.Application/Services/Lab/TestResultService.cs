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
    public class TestResultService : ITestResultService
    {
        private readonly UniCareDbContext _context;

        public TestResultService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<TestResultDto> GetTestResultByIdAsync(Guid id)
        {
            var testResult = await _context.TestResults
                .Include(tr => tr.LabTest)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            return testResult == null ? null : MapToDto(testResult);
        }

        public async Task<IEnumerable<TestResultDto>> GetAllTestResultsAsync()
        {
            return await _context.TestResults
                .Include(tr => tr.LabTest)
                .Select(tr => MapToDto(tr))
                .ToListAsync();
        }

        public async Task<IEnumerable<TestResultDto>> GetTestResultsForRequestAsync(Guid testRequestId)
        {
            return await _context.TestResults
                .Include(tr => tr.LabTest)
                .Where(tr => tr.TestRequestId == testRequestId)
                .Select(tr => MapToDto(tr))
                .ToListAsync();
        }

        public async Task<TestResultDto> CreateTestResultAsync(TestResultDto testResultDto)
        {
            var testResult = new TestResult
            {
                TestRequestId = testResultDto.TestRequestId,
                LabTestId = testResultDto.LabTestId,
                ResultDate = testResultDto.ResultDate,
                ResultValue = testResultDto.ResultValue,
                Units = testResultDto.Units,
                ReferenceRange = testResultDto.ReferenceRange,
                Notes = testResultDto.Notes
            };

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

            return MapToDto(testResult);
        }

        public async Task<TestResultDto> UpdateTestResultAsync(TestResultDto testResultDto)
        {
            var testResult = await _context.TestResults.FindAsync(testResultDto.Id);
            if (testResult == null) return null;

            testResult.ResultDate = testResultDto.ResultDate;
            testResult.ResultValue = testResultDto.ResultValue;
            testResult.Units = testResultDto.Units;
            testResult.ReferenceRange = testResultDto.ReferenceRange;
            testResult.Notes = testResultDto.Notes;

            await _context.SaveChangesAsync();

            return MapToDto(testResult);
        }

        public async Task DeleteTestResultAsync(Guid id)
        {
            var testResult = await _context.TestResults.FindAsync(id);
            if (testResult != null)
            {
                _context.TestResults.Remove(testResult);
                await _context.SaveChangesAsync();
            }
        }

        private static TestResultDto MapToDto(TestResult testResult)
        {
            return new TestResultDto
            {
                Id = testResult.Id,
                TestRequestId = testResult.TestRequestId,
                LabTestId = testResult.LabTestId,
                LabTestName = testResult.LabTest?.Name,
                ResultDate = testResult.ResultDate,
                ResultValue = testResult.ResultValue,
                Units = testResult.Units,
                ReferenceRange = testResult.ReferenceRange,
                Notes = testResult.Notes
            };
        }
    }
}
