using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;

namespace UniCareERP.Application.Services.Lab
{
    public interface ILabTestService
    {
        Task<LabTestDto> GetLabTestByIdAsync(Guid id);
        Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync();
        Task<LabTestDto> CreateLabTestAsync(LabTestDto labTestDto);
        Task<LabTestDto> UpdateLabTestAsync(LabTestDto labTestDto);
        Task DeleteLabTestAsync(Guid id);
    }
}
