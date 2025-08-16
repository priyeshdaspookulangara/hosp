using System;

namespace UniCareERP.Application.DTOs.Vitals
{
    public class VitalDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public string RecordedById { get; set; }
        public string RecordedByName { get; set; }
        public DateTime RecordedAt { get; set; }
        public decimal TemperatureC { get; set; }
        public decimal TemperatureF { get; set; }
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }
        public int HeartRate { get; set; }
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; }
    }
}
