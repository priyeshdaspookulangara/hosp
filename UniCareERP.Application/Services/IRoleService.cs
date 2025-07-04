using UniCareERP.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniCareERP.Application.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(string id);
        Task<(bool Success, string? RoleId, IEnumerable<string> Errors)> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<(bool Success, IEnumerable<string> Errors)> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<(bool Success, IEnumerable<string> Errors)> DeleteRoleAsync(string id);
        Task<RoleDto?> GetRoleByNameAsync(string roleName);
        Task<List<string>> GetAllRoleNamesAsync();
    }
}
