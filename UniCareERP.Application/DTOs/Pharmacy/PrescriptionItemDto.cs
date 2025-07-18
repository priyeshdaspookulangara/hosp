namespace UniCareERP.Application.DTOs.Pharmacy
{
    public class PrescriptionItemDto
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int DrugId { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
    }
}
