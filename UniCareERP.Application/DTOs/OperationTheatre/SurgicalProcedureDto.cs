using System;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class SurgicalProcedureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RequiredEquipment { get; set; }
        public int DurationMinutes { get; set; }
    }
}
