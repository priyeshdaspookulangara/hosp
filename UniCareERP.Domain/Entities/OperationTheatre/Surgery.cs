using System;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.HR;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class Surgery
    {
        public Guid Id { get; set; }
        public string SurgeryName { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public Guid OperationTheatreId { get; set; }
        public OperationTheatre OperationTheatre { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Guid PrimarySurgeonId { get; set; }
        public Employee PrimarySurgeon { get; set; }
        public Guid AnesthetistId { get; set; }
        public Employee Anesthetist { get; set; }
        public string Notes { get; set; }
    }
}
