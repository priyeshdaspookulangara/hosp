using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.PatientDashboard;

namespace UniCareERP.Application.Services.PatientDashboard
{
    public interface IPatientDashboardService
    {
        Task<PatientDashboardDto> GetPatientDashboardAsync(Guid patientId);
    }
}
