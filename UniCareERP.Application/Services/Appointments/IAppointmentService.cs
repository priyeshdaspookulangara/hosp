using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.Services.Appointments
{
    public interface IAppointmentService
    {
        Task<AppointmentDto?> ScheduleAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<AppointmentDto?> GetAppointmentByIdAsync(Guid appointmentId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsForPatientAsync(Guid patientId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsForDoctorAsync(string doctorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate); // For general calendar views
        Task<AppointmentDto?> UpdateAppointmentAsync(UpdateAppointmentDto updateAppointmentDto);
        Task<bool> ChangeAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, string? notes = null);
        Task<bool> CancelAppointmentAsync(Guid appointmentId, string cancellationReason, bool cancelledByPatient);

        // Consider a more specific availability check method if complex logic is needed:
        // Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime appointmentDateTime, int durationMinutes, Guid? excludingAppointmentId = null);
    }
}
