namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class OperationTheatre
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsAvailable { get; set; }
    }
}
