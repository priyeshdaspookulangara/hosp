using System;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Web.ViewModels.Finance
{
    public class AddPaymentViewModel
    {
        public Guid InvoiceId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string TransactionId { get; set; }

        public string Notes { get; set; }
    }
}
