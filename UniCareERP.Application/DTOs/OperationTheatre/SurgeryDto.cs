using System;
using UniCareERP.Domain.Enums.OperationTheatre;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class SurgeryDto
    {
        public Guid Id { get; set; }
        public string SurgeryName { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid OperationTheatreId { get; set; }
        public string OperationTheatreName { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Guid PrimarySurgeonId { get; set; }
        public string PrimarySurgeonName { get; set; }
        public Guid AnesthetistId { get; set; }
        public string AnesthetistName { get; set; }
        public string Notes { get; set; }
        public SurgeryStatus Status { get; set; }
    }
}
