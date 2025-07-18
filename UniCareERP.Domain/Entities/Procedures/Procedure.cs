using System;
using System.Collections.Generic;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Procedures
{
    public class Procedure
    {
        public Guid Id { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
        public Guid? SurgeonId { get; set; } // Assuming surgeons are Employees/Users
        public DateTime ProcedureDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ProcedureStatus Status { get; set; }
        public string? Notes { get; set; }
        public virtual ICollection<ProcedureCharge> ProcedureCharges { get; set; } = new List<ProcedureCharge>();

        public Procedure()
        {
            Id = Guid.NewGuid();
            Status = ProcedureStatus.Scheduled;
        }
    }
}
