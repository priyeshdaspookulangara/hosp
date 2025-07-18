using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Lab;

namespace UniCareERP.Application.Services.Lab
{
    public interface ILabService
    {
        Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync();
        Task<LabTestDto> GetLabTestByIdAsync(Guid id);
        Task<LabTestDto> CreateLabTestAsync(LabTestDto labTestDto);
        Task UpdateLabTestAsync(Guid id, LabTestDto labTestDto);
        Task DeleteLabTestAsync(Guid id);

        Task<IEnumerable<LabOrderDto>> GetAllLabOrdersAsync();
        Task<LabOrderDto> GetLabOrderByIdAsync(Guid id);
        Task<IEnumerable<LabOrderDto>> GetLabOrdersByPatientIdAsync(Guid patientId);
        Task<LabOrderDto> CreateLabOrderAsync(LabOrderDto labOrderDto);
        Task UpdateLabOrderAsync(Guid id, LabOrderDto labOrderDto);
        Task DeleteLabOrderAsync(Guid id);
    }
}
