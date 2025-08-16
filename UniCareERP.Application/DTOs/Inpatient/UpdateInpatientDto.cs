using System;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.Inpatient
{
    public class UpdateInpatientDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string BedNumber { get; set; }

        [Required]
        public string AdmissionReason { get; set; }

        public string DischargeReason { get; set; }
    }
}
