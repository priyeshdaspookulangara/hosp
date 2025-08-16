using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class CreateOTScheduleDto
    {
        [Required]
        public Guid OperationTheatreId { get; set; }
        [Required]
        public Guid SurgicalProcedureId { get; set; }
        [Required]
        public Guid PatientId { get; set; }
        [Required]
        public Guid SurgicalTeamId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
    }
}
