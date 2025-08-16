using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public interface IOperationTheatreService
    {
        Task<IEnumerable<OperationTheatreDto>> GetAllOperationTheatresAsync();
        Task<OperationTheatreDto> GetOperationTheatreByIdAsync(Guid id);
        Task<OperationTheatreDto> CreateOperationTheatreAsync(OperationTheatreDto operationTheatreDto);
        Task UpdateOperationTheatreAsync(Guid id, OperationTheatreDto operationTheatreDto);
        Task DeleteOperationTheatreAsync(Guid id);
    }
}
