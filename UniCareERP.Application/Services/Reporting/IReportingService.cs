using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Reporting;

namespace UniCareERP.Application.Services.Reporting
{
    public interface IReportingService
    {
        Task<IEnumerable<ProcedureReportDto>> GetProcedureReportAsync(DateTime startDate, DateTime endDate);
        Task<FinanceReportDto> GetFinanceReportAsync(DateTime startDate, DateTime endDate);
    }
}
