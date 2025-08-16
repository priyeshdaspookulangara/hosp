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
        Task<OperationTheatreDto> CreateOperationTheatreAsync(CreateOperationTheatreDto createOperationTheatreDto);
        Task<bool> UpdateOperationTheatreAsync(UpdateOperationTheatreDto updateOperationTheatreDto);
        Task<bool> DeleteOperationTheatreAsync(Guid id);

        Task<IEnumerable<SurgicalProcedureDto>> GetAllSurgicalProceduresAsync();
        Task<SurgicalProcedureDto> GetSurgicalProcedureByIdAsync(Guid id);
        Task<SurgicalProcedureDto> CreateSurgicalProcedureAsync(CreateSurgicalProcedureDto createSurgicalProcedureDto);
        Task<bool> UpdateSurgicalProcedureAsync(UpdateSurgicalProcedureDto updateSurgicalProcedureDto);
        Task<bool> DeleteSurgicalProcedureAsync(Guid id);

        Task<IEnumerable<SurgicalTeamDto>> GetAllSurgicalTeamsAsync();
        Task<SurgicalTeamDto> GetSurgicalTeamByIdAsync(Guid id);
        Task<SurgicalTeamDto> CreateSurgicalTeamAsync(CreateSurgicalTeamDto createSurgicalTeamDto);
        Task<bool> UpdateSurgicalTeamAsync(UpdateSurgicalTeamDto updateSurgicalTeamDto);
        Task<bool> DeleteSurgicalTeamAsync(Guid id);
    }
}
