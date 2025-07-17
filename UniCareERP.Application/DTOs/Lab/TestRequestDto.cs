using System;
using System.Collections.Generic;

namespace UniCareERP.Application.DTOs.Lab
{
    public class TestRequestDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime RequestDate { get; set; }
        public ICollection<LabTestDto> LabTests { get; set; } = new List<LabTestDto>();
    }
}
