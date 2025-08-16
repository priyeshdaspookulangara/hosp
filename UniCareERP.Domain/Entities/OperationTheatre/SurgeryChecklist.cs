using System;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class SurgeryChecklist
    {
        public Guid Id { get; set; }
        public Guid SurgeryId { get; set; }
        public Surgery Surgery { get; set; }
        public string ChecklistName { get; set; }
        public bool IsCompleted { get; set; }
    }
}
