using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class OTSchedule
    {
        public Guid Id { get; set; }
        public Guid OperationTheatreId { get; set; }
        public virtual OperationTheatre OperationTheatre { get; set; }
        public Guid SurgicalProcedureId { get; set; }
        public virtual SurgicalProcedure SurgicalProcedure { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public Guid SurgicalTeamId { get; set; }
        public virtual SurgicalTeam SurgicalTeam { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }

        public OTSchedule()
        {
            Id = Guid.NewGuid();
        }
    }
}
