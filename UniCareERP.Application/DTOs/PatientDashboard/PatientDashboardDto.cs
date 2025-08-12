using System;
using System.Collections.Generic;
using UniCareERP.Application.DTOs.Patients;

namespace UniCareERP.Application.DTOs.PatientDashboard
{
    public class PatientDashboardDto
    {
        public PatientDto Patient { get; set; } = null!;
        public IEnumerable<PatientAppointmentSummaryDto> Appointments { get; set; } = new List<PatientAppointmentSummaryDto>();
        public IEnumerable<PatientInvoiceSummaryDto> Invoices { get; set; } = new List<PatientInvoiceSummaryDto>();
        public IEnumerable<PatientPrescriptionSummaryDto> Prescriptions { get; set; } = new List<PatientPrescriptionSummaryDto>();
    }
}
