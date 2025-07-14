using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Inventory
{
    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; } // Selling price
        public decimal CostPrice { get; set; }   // Purchase price
        public int ReorderLevel { get; set; }
        public string? SupplierInfo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateInventoryItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Selling Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        [MaxLength(200)]
        [Display(Name = "Supplier Information")]
        public string? SupplierInfo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Batch Number")]
        public string? BatchNumber { get; set; }

        // Initial stock quantity will be set via a separate stock transaction, not here.
        // public int InitialQuantity { get; set; } = 0;
    }

    public class UpdateInventoryItemDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Selling Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        [MaxLength(200)]
        [Display(Name = "Supplier Information")]
        public string? SupplierInfo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Batch Number")]
        public string? BatchNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
