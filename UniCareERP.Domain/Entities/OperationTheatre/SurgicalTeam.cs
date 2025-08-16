using System;
using System.Collections.Generic;
using UniCareERP.Domain.Entities;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class SurgicalTeam
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }

        public SurgicalTeam()
        {
            Id = Guid.NewGuid();
            Members = new List<ApplicationUser>();
        }
    }
}
