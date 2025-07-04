using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.HR; // Assuming DTOs will be created later

namespace UniCareERP.Application.Services.HR
{
    public interface IHRService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(); // Replace with EmployeeDto later
        Task<Employee?> GetEmployeeByIdAsync(Guid id);       // Replace with EmployeeDto later
        // Add other method signatures (e.g., for Leave Requests, Payroll)
    }
}
