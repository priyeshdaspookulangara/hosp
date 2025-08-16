using System;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class SurgicalProcedure
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RequiredEquipment { get; set; } // Could be a JSON string or a separate entity
        public int DurationMinutes { get; set; }

        public SurgicalProcedure()
        {
            Id = Guid.NewGuid();
        }
    }
}
