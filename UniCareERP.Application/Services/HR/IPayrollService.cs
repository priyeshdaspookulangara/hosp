using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;

namespace UniCareERP.Application.Services.HR
{
    public interface IPayrollService
    {
        Task<PayrollDto> GeneratePayrollAsync(Guid employeeId, DateTime payPeriodStartDate, DateTime payPeriodEndDate);
        Task<PayrollDto> GetPayrollByIdAsync(Guid payrollId);
        Task<IEnumerable<PayrollDto>> GetPayrollsForEmployeeAsync(Guid employeeId);
    }
}
