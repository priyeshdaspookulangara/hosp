using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Lab
{
    public class TestRequest
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }

        public Guid? DoctorId { get; set; } // The doctor who ordered the test

        [Required]
        public DateTime RequestDate { get; set; }

        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    }
}
