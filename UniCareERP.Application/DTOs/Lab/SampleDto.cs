using System;

namespace UniCareERP.Application.DTOs.Lab
{
    public class SampleDto
    {
        public Guid Id { get; set; }
        public Guid TestRequestId { get; set; }
        public string SampleType { get; set; }
        public DateTime CollectionDate { get; set; }
        public string CollectorId { get; set; }
        public string CollectorName { get; set; }
    }
}
