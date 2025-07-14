using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;

namespace UniCareERP.Application.Services.Inventory
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDto?> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createDto);
        Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(Guid poId);
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
        Task<bool> ApprovePurchaseOrderAsync(Guid poId);
        Task<bool> CancelPurchaseOrderAsync(Guid poId);
        Task<PurchaseOrderDto?> ReceiveGoodsAsync(Guid poId, List<ReceivedItemDto> receivedItems);
    }
}
