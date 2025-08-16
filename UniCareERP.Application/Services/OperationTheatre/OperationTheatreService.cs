using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.OperationTheatre;

namespace UniCareERP.Application.Services.OperationTheatre
{
    public class OperationTheatreService : IOperationTheatreService
    {
        public Task<OperationTheatreDto> CreateOperationTheatreAsync(OperationTheatreDto operationTheatreDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOperationTheatreAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OperationTheatreDto>> GetAllOperationTheatresAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OperationTheatreDto> GetOperationTheatreByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOperationTheatreAsync(Guid id, OperationTheatreDto operationTheatreDto)
        {
            throw new NotImplementedException();
        }
    }
}
