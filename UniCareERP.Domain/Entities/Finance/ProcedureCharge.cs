using System;
using System.ComponentModel.DataAnnotations.Schema;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.Procedures;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Domain.Entities.Finance
{
    public class ProcedureCharge
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
        public Guid ProcedureId { get; set; }
        public virtual Procedure? Procedure { get; set; }
        public Guid? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public ChargeType ChargeType { get; set; }
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateTime ChargeDate { get; set; }
        public ChargeStatus Status { get; set; }
        public string? TransactionId { get; set; } // For idempotency

        public ProcedureCharge()
        {
            Id = Guid.NewGuid();
            ChargeDate = DateTime.UtcNow;
            Status = ChargeStatus.Pending;
        }
    }
}
