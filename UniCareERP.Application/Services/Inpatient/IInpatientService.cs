using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inpatient;

namespace UniCareERP.Application.Services.Inpatient
{
    public interface IInpatientService
    {
        Task<IEnumerable<InpatientDto>> GetAllInpatientsAsync();
        Task<InpatientDto> GetInpatientByIdAsync(Guid id);
        Task<InpatientDto> CreateInpatientAsync(CreateInpatientDto createInpatientDto);
        Task<bool> UpdateInpatientAsync(UpdateInpatientDto updateInpatientDto);
        Task<bool> DeleteInpatientAsync(Guid id);
    }
}
