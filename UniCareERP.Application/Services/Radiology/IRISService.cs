using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Radiology;

namespace UniCareERP.Application.Services.Radiology
{
    public interface IRISService
    {
        Task<RadiologyOrderDto> CreateOrderAsync(RadiologyOrderDto orderDto);
        Task<RadiologyOrderDto> GetOrderAsync(Guid orderId);
        Task<IEnumerable<RadiologyOrderDto>> GetOrdersAsync();
        Task<RadiologyReportDto> CreateReportAsync(RadiologyReportDto reportDto);
        Task<RadiologyReportDto> GetReportAsync(Guid reportId);
        Task<IEnumerable<RadiologyReportDto>> GetReportsForOrderAsync(Guid orderId);
        Task<RadiologyImageDto> AddImageToOrderAsync(RadiologyImageDto imageDto);
        Task<IEnumerable<RadiologyImageDto>> GetImagesForOrderAsync(Guid orderId);
        Task<IEnumerable<RadiologyTestDto>> GetTestTypesAsync();
    }
}
