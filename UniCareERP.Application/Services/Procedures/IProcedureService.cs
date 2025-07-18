using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Procedures;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.Services.Procedures
{
    public interface IProcedureService
    {
        Task<ProcedureDto> GetProcedureByIdAsync(Guid id);
        Task<IEnumerable<ProcedureDto>> GetAllProceduresAsync();
        Task<ProcedureDto> CreateProcedureAsync(CreateProcedureDto createProcedureDto);
        Task UpdateProcedureAsync(UpdateProcedureDto updateProcedureDto);
        Task DeleteProcedureAsync(Guid id);
        Task UpdateProcedureStatusAsync(Guid procedureId, ProcedureStatus newStatus);
    }
}
