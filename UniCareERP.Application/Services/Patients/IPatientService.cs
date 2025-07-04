using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Patients; // Assuming DTOs will be created later or entities used directly for now

namespace UniCareERP.Application.Services.Patients
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(); // Replace Patient with PatientDto later
        Task<Patient?> GetPatientByIdAsync(Guid id);      // Replace Patient with PatientDto later
        // Add other method signatures like Create, Update, Delete
    }
}
