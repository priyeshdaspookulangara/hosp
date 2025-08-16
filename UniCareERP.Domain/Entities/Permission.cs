using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Domain.Entities
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        [Required]
        public string Module { get; set; } // e.g., "Patients", "Invoices"

        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}
