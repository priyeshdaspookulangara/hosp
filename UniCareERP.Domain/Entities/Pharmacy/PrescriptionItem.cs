namespace UniCareERP.Domain.Entities.Pharmacy
{
    public class PrescriptionItem
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; } = null!;
        public int DrugId { get; set; }
        public Drug Drug { get; set; } = null!;
        public string Dosage { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
