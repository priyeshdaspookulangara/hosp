using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Application.Services.Radiology
{
    public interface IRadiologyReportService
    {
        Task<IEnumerable<RadiologyReport>> GetAllRadiologyReportsAsync();
        Task<RadiologyReport> GetRadiologyReportByIdAsync(Guid id);
        Task AddRadiologyReportAsync(RadiologyReport radiologyReport);
        Task UpdateRadiologyReportAsync(RadiologyReport radiologyReport);
        Task DeleteRadiologyReportAsync(Guid id);
    }
}
