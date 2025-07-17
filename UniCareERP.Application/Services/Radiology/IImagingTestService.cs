using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Application.Services.Radiology
{
    public interface IImagingTestService
    {
        Task<IEnumerable<ImagingTest>> GetAllImagingTestsAsync();
        Task<ImagingTest> GetImagingTestByIdAsync(Guid id);
        Task AddImagingTestAsync(ImagingTest imagingTest);
        Task UpdateImagingTestAsync(ImagingTest imagingTest);
        Task DeleteImagingTestAsync(Guid id);
    }
}
