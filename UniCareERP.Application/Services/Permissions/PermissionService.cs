using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.Permissions;
using UniCareERP.Domain.Entities;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly UniCareDbContext _context;

        public PermissionService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsForRoleAsync(string roleId)
        {
            return await _context.Permissions
                .Where(p => p.RoleId == roleId)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    RoleId = p.RoleId,
                    Module = p.Module,
                    CanRead = p.CanRead,
                    CanCreate = p.CanCreate,
                    CanUpdate = p.CanUpdate,
                    CanDelete = p.CanDelete
                })
                .ToListAsync();
        }

        public async Task<bool> UpdatePermissionsForRoleAsync(UpdatePermissionsDto updatePermissionsDto)
        {
            var existingPermissions = await _context.Permissions
                .Where(p => p.RoleId == updatePermissionsDto.RoleId)
                .ToListAsync();

            _context.Permissions.RemoveRange(existingPermissions);

            var newPermissions = updatePermissionsDto.Permissions.Select(p => new Permission
            {
                RoleId = updatePermissionsDto.RoleId,
                Module = p.Module,
                CanRead = p.CanRead,
                CanCreate = p.CanCreate,
                CanUpdate = p.CanUpdate,
                CanDelete = p.CanDelete
            });

            await _context.Permissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
