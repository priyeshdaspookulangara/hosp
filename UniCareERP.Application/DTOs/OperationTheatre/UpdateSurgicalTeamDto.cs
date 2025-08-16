using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.OperationTheatre
{
    public class UpdateSurgicalTeamDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IEnumerable<string> MemberIds { get; set; }
    }
}
