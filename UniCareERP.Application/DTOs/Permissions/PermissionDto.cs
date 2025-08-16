using System;

namespace UniCareERP.Application.DTOs.Permissions
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string RoleId { get; set; }
        public string Module { get; set; }
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
