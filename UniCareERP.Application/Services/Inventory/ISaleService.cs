using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;

namespace UniCareERP.Application.Services.Inventory
{
    public interface ISaleService
    {
        Task<SaleDto?> CreateSaleAsync(CreateSaleDto createDto);
        Task<SaleDto?> GetSaleByIdAsync(Guid saleId);
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
    }
}
