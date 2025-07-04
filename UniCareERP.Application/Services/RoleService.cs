using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs;
using UniCareERP.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniCareERP.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<(bool Success, string? RoleId, IEnumerable<string> Errors)> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
            {
                return (false, null, new[] { $"Role '{createRoleDto.Name}' already exists." });
            }

            var role = new ApplicationRole(createRoleDto.Name, createRoleDto.Description);
            var result = await _roleManager.CreateAsync(role);

            return (result.Succeeded, role.Id, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return (false, new[] { "Role not found." });
            }

            // Consider checking if users are in this role before deleting
            // var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            // if (usersInRole.Any()) return (false, new[] { "Cannot delete role with assigned users." });

            var result = await _roleManager.DeleteAsync(role);
            return (result.Succeeded, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name ?? "N/A", Description = r.Description })
                .ToListAsync();
        }

        public async Task<List<string>> GetAllRoleNamesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToListAsync();
        }


        public async Task<RoleDto?> GetRoleByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return null;
            return new RoleDto { Id = role.Id, Name = role.Name ?? "N/A", Description = role.Description };
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return null;
            return new RoleDto { Id = role.Id, Name = role.Name ?? "N/A", Description = role.Description };
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var role = await _roleManager.FindByIdAsync(updateRoleDto.Id);
            if (role == null)
            {
                return (false, new[] { "Role not found." });
            }

            // Check if new role name conflicts with an existing role (if name is changed)
            if (role.Name != updateRoleDto.Name && await _roleManager.RoleExistsAsync(updateRoleDto.Name))
            {
                 return (false, new[] { $"Role '{updateRoleDto.Name}' already exists." });
            }

            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;

            var result = await _roleManager.UpdateAsync(role);
            return (result.Succeeded, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }
    }
}
