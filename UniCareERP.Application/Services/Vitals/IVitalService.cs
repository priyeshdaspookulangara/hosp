using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Vitals;

namespace UniCareERP.Application.Services.Vitals
{
    public interface IVitalService
    {
        Task<IEnumerable<VitalDto>> GetVitalsForPatientAsync(Guid patientId);
        Task<VitalDto> GetVitalByIdAsync(Guid id);
        Task<VitalDto> CreateVitalAsync(VitalDto vitalDto);
        Task UpdateVitalAsync(Guid id, VitalDto vitalDto);
        Task DeleteVitalAsync(Guid id);
    }
}
