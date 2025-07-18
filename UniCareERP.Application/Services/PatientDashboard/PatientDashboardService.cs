using System;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.PatientDashboard;
using UniCareERP.Application.Services.Appointments;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.Patients;

using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.DTOs.Patients;
using UniCareERP.Domain.Entities.Users;
using UniCareERP.Domain.Interfaces;

namespace UniCareERP.Application.Services.PatientDashboard
{
    public class PatientDashboardService : IPatientDashboardService
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPrescriptionService _prescriptionService;

        public PatientDashboardService(
            IPatientService patientService,
            IAppointmentService appointmentService,
            IInvoiceService invoiceService,
            IPrescriptionService prescriptionService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _invoiceService = invoiceService;
            _prescriptionService = prescriptionService;
        }

        public async Task<PatientDashboardDto> GetPatientDashboardAsync(Guid patientId)
        {
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                return null;
            }

            var appointments = await _appointmentService.GetAppointmentsForPatientAsync(patientId);
            var invoices = await _invoiceService.GetInvoicesForPatientAsync(patientId);
            var prescriptions = await _prescriptionService.GetPrescriptionsForPatientAsync(patientId);

            var dashboardDto = new PatientDashboardDto
            {
                Patient = patient,
                Appointments = appointments.Select(a => new PatientAppointmentSummaryDto
                {
                    Id = a.Id,
                    AppointmentDateTime = a.AppointmentDateTime,
                    DoctorName = a.Doctor.FirstName,
                    ServiceType = a.ServiceType,
                    Status = a.Status
                }),
                Invoices = invoices.Select(i => new PatientInvoiceSummaryDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    AmountPaid = i.AmountPaid,
                    Status = i.Status
                }),
                Prescriptions = prescriptions.Select(p => new PatientPrescriptionSummaryDto
                {
                    Id = p.Id,
                    PrescriptionDate = p.PrescriptionDate,
                    DoctorName = p.Doctor.FirstName,
                    Status = p.Status,
                    NumberOfItems = p.Items.Count()
                })
            };

            return dashboardDto;
        }
    }
}
