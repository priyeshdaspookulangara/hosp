using System;
using System.Collections.Generic;
using UniCareERP.Application.DTOs;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class SurgicalTeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<UserDto> Members { get; set; }
    }
}
