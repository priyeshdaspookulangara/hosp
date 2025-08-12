using System;
using UniCareERP.Domain.Entities.Users;

namespace UniCareERP.Domain.Entities.Schedules
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
