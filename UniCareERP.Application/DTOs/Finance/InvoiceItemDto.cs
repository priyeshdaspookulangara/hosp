using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Finance
{
    public class InvoiceItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreateInvoiceItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 1000000)]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
    }
}
