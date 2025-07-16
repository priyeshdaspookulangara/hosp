using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;

namespace UniCareERP.Application.Services.Patients
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto?> CreatePrescriptionAsync(CreatePrescriptionDto createDto, string doctorId);
        Task<PrescriptionDto?> GetPrescriptionByIdAsync(Guid prescriptionId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsForPatientAsync(Guid patientId);
        Task<bool> CancelPrescriptionAsync(Guid prescriptionId, string userId); // userId to verify ownership/permission
    }
}
