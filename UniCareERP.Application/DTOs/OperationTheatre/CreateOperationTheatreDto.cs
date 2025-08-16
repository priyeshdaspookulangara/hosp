using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class CreateOperationTheatreDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string RoomNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string Location { get; set; }
        public string Equipment { get; set; }
    }
}
