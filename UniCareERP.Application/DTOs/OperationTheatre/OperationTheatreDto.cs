using System;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class OperationTheatreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RoomNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string Location { get; set; }
        public string Equipment { get; set; }
    }
}
