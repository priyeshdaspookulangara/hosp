using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;

namespace UniCareERP.Application.Services.HR
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto createDto);
        Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<UpdateEmployeeDto?> GetEmployeeForUpdateAsync(Guid employeeId);
        Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeDto updateDto);
        Task<bool> DeactivateEmployeeAsync(Guid employeeId);
        Task<bool> ReactivateEmployeeAsync(Guid employeeId);
        Task<bool> DeleteEmployeeAsync(Guid employeeId);
    }
}
