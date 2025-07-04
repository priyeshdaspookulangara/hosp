using Microsoft.AspNetCore.Identity;

namespace UniCareERP.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        // You can add custom properties to roles if needed
        public string? Description { get; set; }

        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
        public ApplicationRole(string roleName, string? description) : base(roleName)
        {
            Description = description;
        }
    }
}
