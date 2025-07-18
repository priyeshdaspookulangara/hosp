using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniCareERP.Domain.Entities.Lab
{
    public class LabTest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public string NormalRange { get; set; } = string.Empty;

        public LabTest()
        {
            Id = Guid.NewGuid();
        }
    }
}
