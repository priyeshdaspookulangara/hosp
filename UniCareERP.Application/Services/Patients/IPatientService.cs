using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;

namespace UniCareERP.Application.Services.Patients
{
    public interface IPatientService
    {
        Task<PatientDto?> CreatePatientAsync(CreatePatientDto createPatientDto);
        Task<PatientDto?> GetPatientByIdAsync(Guid id);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<bool> UpdatePatientAsync(UpdatePatientDto updatePatientDto);
        Task<bool> DeletePatientAsync(Guid id); // True if successful, false otherwise
        Task<string> GenerateNextPatientCodeAsync();
    }
}
