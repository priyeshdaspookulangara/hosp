using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Inventory
{
    public class PurchaseOrderItemDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityReceived { get; set; }
        public decimal TotalPrice => QuantityOrdered * UnitPrice;
    }

    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public string? SupplierInfo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public Guid InventoryItemId { get; set; }

        [Required]
        [Range(1, 10000)]
        public int QuantityOrdered { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; } // Cost price
    }

    public class CreatePurchaseOrderDto
    {
        [MaxLength(200)]
        public string? SupplierInfo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Purchase Order must have at least one item.")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new List<CreatePurchaseOrderItemDto>();
    }

    // DTO for receiving goods against a PO
    public class ReceivedItemDto
    {
        [Required]
        public Guid PurchaseOrderItemId { get; set; }

        [Required]
        [Range(1, 10000)]
        public int QuantityReceived { get; set; }
    }
}
