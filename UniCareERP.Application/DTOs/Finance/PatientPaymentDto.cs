using System;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Finance
{
    public class PatientPaymentDto
    {
        public Guid PatientId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
