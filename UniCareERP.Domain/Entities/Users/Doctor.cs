using System.Collections.Generic;
using UniCareERP.Domain.Entities.Schedules;

namespace UniCareERP.Domain.Entities.Users
{
    public class Doctor : ApplicationUser
    {
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
