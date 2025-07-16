using System;
using UniCareERP.Domain.Entities.Inventory;

namespace UniCareERP.Domain.Entities.Patients
{
    public class PrescriptionItem
    {
        public Guid Id { get; set; }

        public Guid PrescriptionId { get; set; }
        public virtual Prescription? Prescription { get; set; }

        public Guid InventoryItemId { get; set; } // Links to the drug/item
        public virtual InventoryItem? InventoryItem { get; set; }

        public string Dosage { get; set; } = string.Empty; // e.g., "500mg"
        public string Frequency { get; set; } = string.Empty; // e.g., "Twice a day"
        public string Duration { get; set; } = string.Empty; // e.g., "For 7 days"
        public string? Notes { get; set; }

        // Could also add fields for quantity to dispense, refills, etc.
        // public int QuantityToDispense { get; set; }

        public PrescriptionItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
