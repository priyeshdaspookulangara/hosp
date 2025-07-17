using System;
using System.Collections.Generic;

namespace UniCareERP.Domain.Entities.Radiology
{
    public class ImagingService
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
