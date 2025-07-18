namespace UniCareERP.Domain.Entities.Pharmacy
{
    public class PrescriptionItem
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public int DrugId { get; set; }
        public Drug Drug { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
    }
}
