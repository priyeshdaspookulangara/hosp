using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Permissions;

namespace UniCareERP.Application.Services.Permissions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetPermissionsForRoleAsync(string roleId);
        Task<bool> UpdatePermissionsForRoleAsync(UpdatePermissionsDto updatePermissionsDto);
    }
}
