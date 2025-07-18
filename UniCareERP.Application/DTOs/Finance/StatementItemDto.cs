using System;

namespace UniCareERP.Application.DTOs.Finance
{
    public class StatementItemDto
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Charge { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
