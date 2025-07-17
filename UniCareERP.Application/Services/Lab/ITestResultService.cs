using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;

namespace UniCareERP.Application.Services.Lab
{
    public interface ITestResultService
    {
        Task<TestResultDto> GetTestResultByIdAsync(Guid id);
        Task<IEnumerable<TestResultDto>> GetAllTestResultsAsync();
        Task<IEnumerable<TestResultDto>> GetTestResultsForRequestAsync(Guid testRequestId);
        Task<TestResultDto> CreateTestResultAsync(TestResultDto testResultDto);
        Task<TestResultDto> UpdateTestResultAsync(TestResultDto testResultDto);
        Task DeleteTestResultAsync(Guid id);
    }
}
