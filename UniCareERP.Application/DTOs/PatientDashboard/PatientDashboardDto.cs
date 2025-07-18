using System;
using System.Collections.Generic;
using UniCareERP.Application.DTOs.Patients;

namespace UniCareERP.Application.DTOs.PatientDashboard
{
    public class PatientDashboardDto
    {
        public PatientDto Patient { get; set; }
        public IEnumerable<PatientAppointmentSummaryDto> Appointments { get; set; }
        public IEnumerable<PatientInvoiceSummaryDto> Invoices { get; set; }
        public IEnumerable<PatientPrescriptionSummaryDto> Prescriptions { get; set; }
    }
}
