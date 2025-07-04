using Microsoft.AspNetCore.Identity;

namespace UniCareERP.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; } // Could be relevant for linking to HR module
        public bool IsActive { get; set; } = true;
        // Add other custom properties for a user here
    }
}
