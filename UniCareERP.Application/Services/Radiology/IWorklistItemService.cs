using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Application.Services.Radiology
{
    public interface IWorklistItemService
    {
        Task<IEnumerable<WorklistItem>> GetAllWorklistItemsAsync();
        Task<WorklistItem> GetWorklistItemByIdAsync(Guid id);
        Task AddWorklistItemAsync(WorklistItem worklistItem);
        Task UpdateWorklistItemAsync(WorklistItem worklistItem);
        Task DeleteWorklistItemAsync(Guid id);
    }
}
