using System;
using System.Collections.Generic;

namespace UniCareERP.Domain.Entities.OperationTheatre
{
    public class OperationTheatre
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RoomNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string Location { get; set; }
        public string Equipment { get; set; } // Could be a JSON string or a separate entity

        public OperationTheatre()
        {
            Id = Guid.NewGuid();
            IsAvailable = true;
        }
    }
}
