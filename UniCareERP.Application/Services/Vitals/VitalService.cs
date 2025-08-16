using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Vitals;

namespace UniCareERP.Application.Services.Vitals
{
    public class VitalService : IVitalService
    {
        public Task<VitalDto> CreateVitalAsync(VitalDto vitalDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVitalAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<VitalDto> GetVitalByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VitalDto>> GetVitalsForPatientAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVitalAsync(Guid id, VitalDto vitalDto)
        {
            throw new NotImplementedException();
        }
    }
}
