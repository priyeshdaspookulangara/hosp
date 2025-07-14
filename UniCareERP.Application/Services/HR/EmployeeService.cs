using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.HR
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UniCareDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            UniCareDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<EmployeeService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<string> GenerateNextEmployeeCodeAsync()
        {
            const string prefix = "EMP";
            var lastEmployee = await _context.Employees
                                             .OrderByDescending(e => e.EmployeeCode)
                                             .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastEmployee != null && lastEmployee.EmployeeCode.StartsWith(prefix))
            {
                string lastNumberStr = lastEmployee.EmployeeCode.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{prefix}{nextNumber:D4}"; // e.g., EMP0001
        }

        public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto createDto)
        {
            if (await _userManager.FindByEmailAsync(createDto.Email) != null)
            {
                _logger.LogWarning($"Attempted to create employee with existing email: {createDto.Email}");
                // Consider returning a more specific error result object
                return null;
            }

            var user = new ApplicationUser
            {
                UserName = createDto.Email,
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                IsActive = createDto.IsActive,
                EmailConfirmed = true // Or false if you have an email confirmation flow
            };

            // This service is transactional. If any part fails, the whole operation should be rolled back.
            // Using a transaction scope is best practice here.
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Create the Identity User
                var identityResult = await _userManager.CreateAsync(user, createDto.Password);
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Failed to create ApplicationUser for {createDto.Email}: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    return null;
                }

                // Step 2: Add roles to the user
                if (createDto.Roles.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, createDto.Roles);
                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                         _logger.LogError($"Failed to add roles to user {createDto.Email}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        return null;
                    }
                }

                // Step 3: Create the Employee record
                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = user.Id,
                    EmployeeCode = await GenerateNextEmployeeCodeAsync(),
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    DateOfBirth = createDto.DateOfBirth,
                    Gender = createDto.Gender,
                    HireDate = createDto.HireDate,
                    JobTitle = createDto.JobTitle,
                    Department = createDto.Department,
                    PhoneNumber = createDto.PhoneNumber,
                    Email = createDto.Email, // Personal email, can be same as system email
                    Address = createDto.Address,
                    IsActive = createDto.IsActive
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Step 4: Commit transaction
                await transaction.CommitAsync();
                _logger.LogInformation($"Employee {employee.EmployeeCode} created successfully for user {user.Email}.");

                return MapEmployeeToDto(employee, user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"An error occurred during the creation of employee {createDto.Email}.");
                // Clean up created user if employee creation failed after user creation
                var createdUser = await _userManager.FindByEmailAsync(createDto.Email);
                if(createdUser != null) await _userManager.DeleteAsync(createdUser);

                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                                          .Include(e => e.ApplicationUser)
                                          .OrderBy(e => e.LastName)
                                          .ThenBy(e => e.FirstName)
                                          .ToListAsync();

            return employees.Select(e => MapEmployeeToDto(e, e.ApplicationUser));
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId)
        {
            var employee = await _context.Employees
                                         .Include(e => e.ApplicationUser)
                                         .FirstOrDefaultAsync(e => e.Id == employeeId);

            return employee == null ? null : MapEmployeeToDto(employee, employee.ApplicationUser);
        }

        public async Task<UpdateEmployeeDto?> GetEmployeeForUpdateAsync(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return null;
            return new UpdateEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                HireDate = employee.HireDate,
                JobTitle = employee.JobTitle,
                Department = employee.Department,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeDto updateDto)
        {
            var employee = await _context.Employees.Include(e => e.ApplicationUser).FirstOrDefaultAsync(e => e.Id == updateDto.Id);
            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {updateDto.Id} not found for update.");
                return null;
            }

            // Update Employee fields
            employee.FirstName = updateDto.FirstName;
            employee.LastName = updateDto.LastName;
            employee.DateOfBirth = updateDto.DateOfBirth;
            employee.Gender = updateDto.Gender;
            employee.HireDate = updateDto.HireDate;
            employee.JobTitle = updateDto.JobTitle;
            employee.Department = updateDto.Department;
            employee.PhoneNumber = updateDto.PhoneNumber;
            employee.Address = updateDto.Address;
            employee.IsActive = updateDto.IsActive;

            // Also update linked ApplicationUser
            if (employee.ApplicationUser != null)
            {
                employee.ApplicationUser.FirstName = updateDto.FirstName;
                employee.ApplicationUser.LastName = updateDto.LastName;
                employee.ApplicationUser.IsActive = updateDto.IsActive;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Employee {employee.EmployeeCode} updated successfully.");
                return MapEmployeeToDto(employee, employee.ApplicationUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee ID {updateDto.Id}.");
                return null;
            }
        }

        public async Task<bool> DeactivateEmployeeAsync(Guid employeeId)
        {
            return await SetEmployeeStatusAsync(employeeId, false);
        }

        public async Task<bool> ReactivateEmployeeAsync(Guid employeeId)
        {
            return await SetEmployeeStatusAsync(employeeId, true);
        }

        private async Task<bool> SetEmployeeStatusAsync(Guid employeeId, bool isActive)
        {
            var employee = await _context.Employees.Include(e => e.ApplicationUser).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null) return false;

            employee.IsActive = isActive;
            if(employee.ApplicationUser != null)
            {
                employee.ApplicationUser.IsActive = isActive;
            }

            try
            {
                await _context.SaveChangesAsync();
                 _logger.LogInformation($"Employee {employee.EmployeeCode} status set to {(isActive ? "Active" : "Inactive")}.");
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for employee {employee.EmployeeCode}.");
                return false;
            }
        }


        private static EmployeeDto MapEmployeeToDto(Employee employee, ApplicationUser? user)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = $"{employee.FirstName} {employee.LastName}",
                JobTitle = employee.JobTitle,
                Department = employee.Department,
                Email = user?.Email,
                PhoneNumber = employee.PhoneNumber,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive
            };
        }
    }
}
