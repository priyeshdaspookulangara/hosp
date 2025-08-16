using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Permissions
{
    public class UpdatePermissionsDto
    {
        public string RoleId { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}
