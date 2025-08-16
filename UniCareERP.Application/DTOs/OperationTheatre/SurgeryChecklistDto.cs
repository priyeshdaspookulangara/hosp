using System;
using UniCareERP.Domain.Enums.OperationTheatre;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class SurgeryChecklistDto
    {
        public Guid Id { get; set; }
        public Guid SurgeryId { get; set; }
        public string ChecklistName { get; set; }
        public ChecklistStatus Status { get; set; }
    }
}
