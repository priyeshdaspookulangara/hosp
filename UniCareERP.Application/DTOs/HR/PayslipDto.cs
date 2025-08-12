using System;

namespace UniCareERP.Application.DTOs.HR
{
    public class PayslipDto
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
