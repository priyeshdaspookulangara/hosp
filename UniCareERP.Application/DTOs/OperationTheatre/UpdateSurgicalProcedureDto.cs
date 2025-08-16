using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class UpdateSurgicalProcedureDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string RequiredEquipment { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive number.")]
        public int DurationMinutes { get; set; }
    }
}
