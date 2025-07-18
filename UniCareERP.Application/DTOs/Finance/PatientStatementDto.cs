using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Finance
{
    public class PatientStatementDto
    {
        public Guid PatientId { get; set; }
        public DateTime StatementDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<StatementItemDto> Items { get; set; }
    }
}
