using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;

namespace UniCareERP.Application.Services.Lab
{
    public interface ITestRequestService
    {
        Task<TestRequestDto> GetTestRequestByIdAsync(Guid id);
        Task<IEnumerable<TestRequestDto>> GetAllTestRequestsAsync();
        Task<TestRequestDto> CreateTestRequestAsync(TestRequestDto testRequestDto);
        Task<TestRequestDto> UpdateTestRequestAsync(TestRequestDto testRequestDto);
        Task DeleteTestRequestAsync(Guid id);
    }
}
