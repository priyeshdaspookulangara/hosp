using System;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class RadiologyTest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // e.g., "Chest X-Ray", "CT Scan of Abdomen"
        public string Description { get; set; }
    }
}
