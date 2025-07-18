using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Finance
{
    public class PatientAccount
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCharges { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPayments { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalRefunds { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; set; }
    }
}
