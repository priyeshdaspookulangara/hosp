using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Application.Services.Radiology
{
    public interface IImagingService
    {
        Task<IEnumerable<ImagingService>> GetAllImagingServicesAsync();
        Task<ImagingService> GetImagingServiceByIdAsync(Guid id);
        Task AddImagingServiceAsync(ImagingService imagingService);
        Task UpdateImagingServiceAsync(ImagingService imagingService);
        Task DeleteImagingServiceAsync(Guid id);
    }
}
