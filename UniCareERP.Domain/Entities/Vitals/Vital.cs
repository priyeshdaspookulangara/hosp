using System;
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Domain.Entities.Vitals
{
    public class Vital
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public string RecordedById { get; set; }
        public ApplicationUser RecordedBy { get; set; }

        public DateTime RecordedAt { get; set; }

        public decimal Temperature { get; set; } // in Celsius
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }
        public int HeartRate { get; set; } // Pulse
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; } // SpO2
    }
}
