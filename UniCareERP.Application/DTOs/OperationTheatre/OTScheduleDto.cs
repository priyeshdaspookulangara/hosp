using System;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class OTScheduleDto
    {
        public Guid Id { get; set; }
        public Guid OperationTheatreId { get; set; }
        public string OperationTheatreName { get; set; }
        public Guid SurgicalProcedureId { get; set; }
        public string SurgicalProcedureName { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid SurgicalTeamId { get; set; }
        public string SurgicalTeamName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
    }
}
