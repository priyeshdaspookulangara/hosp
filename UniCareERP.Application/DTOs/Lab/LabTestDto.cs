using System;

namespace UniCareERP.Application.DTOs.Lab
{
    public class LabTestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string NormalRange { get; set; } = string.Empty;
    }
}
