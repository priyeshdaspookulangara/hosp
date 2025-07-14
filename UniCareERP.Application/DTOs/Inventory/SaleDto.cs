using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Inventory
{
    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public Guid? PatientId { get; set; }
        public string PatientName { get; set; } = "Walk-in Customer";
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodText => PaymentMethod.ToString();
        public string? Notes { get; set; }
        public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();
    }

    public class CreateSaleItemDto
    {
        [Required]
        public Guid InventoryItemId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; } // Should be pre-filled from item master but can be overridden
    }

    public class CreateSaleDto
    {
        public Guid? PatientId { get; set; } // Null for walk-ins

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A sale must have at least one item.")]
        public List<CreateSaleItemDto> Items { get; set; } = new List<CreateSaleItemDto>();
    }
}
