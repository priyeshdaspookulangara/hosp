using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly UniCareDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(UniCareDbContext context, UserManager<ApplicationUser> userManager, ILogger<AppointmentService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AppointmentDto?> ScheduleAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            // Validate Patient
            var patient = await _context.Patients.FindAsync(createAppointmentDto.PatientId);
            if (patient == null)
            {
                _logger.LogWarning($"Patient with ID {createAppointmentDto.PatientId} not found.");
                // Optionally return a result object with specific error messages
                return null;
            }

            // Validate Doctor
            var doctor = await _userManager.FindByIdAsync(createAppointmentDto.DoctorId ?? "");
            if (doctor == null || !(await _userManager.IsInRoleAsync(doctor, "Doctor"))) // Assuming "Doctor" role exists
            {
                _logger.LogWarning($"Doctor with ID {createAppointmentDto.DoctorId} not found or is not a doctor.");
                return null;
            }

            // Basic conflict check for the doctor
            DateTime proposedStartTime = createAppointmentDto.AppointmentDateTime;
            DateTime proposedEndTime = proposedStartTime.AddMinutes(createAppointmentDto.DurationMinutes);

            bool isConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorId == createAppointmentDto.DoctorId &&
                               a.Status != AppointmentStatus.CancelledByClinic && // Consider only active/confirmed appts
                               a.Status != AppointmentStatus.CancelledByPatient &&
                               a.Status != AppointmentStatus.Completed && // Or even completed if it matters for buffer
                               a.Status != AppointmentStatus.NoShow &&
                               ((proposedStartTime >= a.AppointmentDateTime && proposedStartTime < DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes)) || // Starts within existing
                                (proposedEndTime > a.AppointmentDateTime && proposedEndTime <= DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes)) || // Ends within existing
                                (proposedStartTime <= a.AppointmentDateTime && proposedEndTime >= DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes))) // Engulfs existing
                               );

            if (isConflict)
            {
                _logger.LogWarning($"Appointment conflict for Doctor ID {createAppointmentDto.DoctorId} at {createAppointmentDto.AppointmentDateTime}.");
                // Consider returning a more specific error to the caller
                return null;
            }


            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = createAppointmentDto.PatientId,
                DoctorId = createAppointmentDto.DoctorId,
                AppointmentDateTime = createAppointmentDto.AppointmentDateTime,
                DurationMinutes = createAppointmentDto.DurationMinutes,
                ServiceType = createAppointmentDto.ServiceType,
                Reason = createAppointmentDto.Reason,
                Notes = createAppointmentDto.Notes,
                Status = AppointmentStatus.Scheduled, // Initial status
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Appointment ID {appointment.Id} scheduled for Patient ID {appointment.PatientId} with Doctor ID {appointment.DoctorId}.");
                return await MapAppointmentToDtoAsync(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scheduling appointment.");
                return null;
            }
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments
                                        .Include(a => a.Patient)
                                        .Include(a => a.Doctor) // ApplicationUser
                                        .FirstOrDefaultAsync(a => a.Id == appointmentId);

            return appointment == null ? null : await MapAppointmentToDtoAsync(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsForPatientAsync(Guid patientId)
        {
            var appointments = await _context.Appointments
                                        .Where(a => a.PatientId == patientId)
                                        .Include(a => a.Patient)
                                        .Include(a => a.Doctor)
                                        .OrderBy(a => a.AppointmentDateTime)
                                        .ToListAsync();
            return await Task.WhenAll(appointments.Select(MapAppointmentToDtoAsync));
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsForDoctorAsync(string doctorId, DateTime startDate, DateTime endDate)
        {
            var appointments = await _context.Appointments
                                        .Where(a => a.DoctorId == doctorId &&
                                                    a.AppointmentDateTime >= startDate &&
                                                    a.AppointmentDateTime < endDate.AddDays(1)) // Include whole end day
                                        .Include(a => a.Patient)
                                        .Include(a => a.Doctor)
                                        .OrderBy(a => a.AppointmentDateTime)
                                        .ToListAsync();
            return await Task.WhenAll(appointments.Select(MapAppointmentToDtoAsync));
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
             var appointments = await _context.Appointments
                                        .Where(a => a.AppointmentDateTime >= startDate &&
                                                    a.AppointmentDateTime < endDate.AddDays(1))
                                        .Include(a => a.Patient)
                                        .Include(a => a.Doctor)
                                        .OrderBy(a => a.AppointmentDateTime)
                                        .ToListAsync();
            return await Task.WhenAll(appointments.Select(MapAppointmentToDtoAsync));
        }


        public async Task<AppointmentDto?> UpdateAppointmentAsync(UpdateAppointmentDto updateAppointmentDto)
        {
            var appointment = await _context.Appointments.FindAsync(updateAppointmentDto.Id);
            if (appointment == null)
            {
                _logger.LogWarning($"Appointment with ID {updateAppointmentDto.Id} not found for update.");
                return null;
            }

            // Validate Patient and Doctor if they are being changed (though typically PatientId isn't changed)
            if (appointment.PatientId != updateAppointmentDto.PatientId)
            {
                 var patient = await _context.Patients.FindAsync(updateAppointmentDto.PatientId);
                 if (patient == null) { _logger.LogWarning($"Patient ID {updateAppointmentDto.PatientId} not found."); return null; }
            }
            if (appointment.DoctorId != updateAppointmentDto.DoctorId)
            {
                var doctor = await _userManager.FindByIdAsync(updateAppointmentDto.DoctorId ?? "");
                if (doctor == null || !(await _userManager.IsInRoleAsync(doctor, "Doctor")))
                { _logger.LogWarning($"Doctor ID {updateAppointmentDto.DoctorId} not found or invalid."); return null; }
            }

            // Basic conflict check if date/time or doctor changed
            if (appointment.AppointmentDateTime != updateAppointmentDto.AppointmentDateTime ||
                appointment.DoctorId != updateAppointmentDto.DoctorId ||
                appointment.DurationMinutes != updateAppointmentDto.DurationMinutes)
            {
                DateTime proposedStartTime = updateAppointmentDto.AppointmentDateTime;
                DateTime proposedEndTime = proposedStartTime.AddMinutes(updateAppointmentDto.DurationMinutes);

                bool isConflict = await _context.Appointments
                    .AnyAsync(a => a.Id != appointment.Id && // Exclude current appointment
                                a.DoctorId == updateAppointmentDto.DoctorId &&
                                a.Status != AppointmentStatus.CancelledByClinic &&
                                a.Status != AppointmentStatus.CancelledByPatient &&
                                a.Status != AppointmentStatus.Completed &&
                                a.Status != AppointmentStatus.NoShow &&
                                ((proposedStartTime >= a.AppointmentDateTime && proposedStartTime < DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes)) ||
                                 (proposedEndTime > a.AppointmentDateTime && proposedEndTime <= DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes)) ||
                                 (proposedStartTime <= a.AppointmentDateTime && proposedEndTime >= DbFunctions.AddMinutes(a.AppointmentDateTime, a.DurationMinutes)))
                                );
                if (isConflict) { _logger.LogWarning($"Update creates conflict for Doctor ID {updateAppointmentDto.DoctorId}."); return null; }
            }


            appointment.PatientId = updateAppointmentDto.PatientId;
            appointment.DoctorId = updateAppointmentDto.DoctorId;
            appointment.AppointmentDateTime = updateAppointmentDto.AppointmentDateTime;
            appointment.DurationMinutes = updateAppointmentDto.DurationMinutes;
            appointment.ServiceType = updateAppointmentDto.ServiceType;
            appointment.Reason = updateAppointmentDto.Reason;
            appointment.Status = updateAppointmentDto.Status;
            appointment.Notes = updateAppointmentDto.Notes;
            appointment.LastModifiedDate = DateTime.UtcNow;

            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Appointment ID {appointment.Id} updated.");
                // Re-fetch with includes to populate navigation properties for DTO mapping
                var updatedAppointmentWithNav = await _context.Appointments
                                                        .Include(a => a.Patient)
                                                        .Include(a => a.Doctor)
                                                        .FirstOrDefaultAsync(a => a.Id == appointment.Id);
                return updatedAppointmentWithNav == null ? null : await MapAppointmentToDtoAsync(updatedAppointmentWithNav);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating appointment ID {appointment.Id}.");
                return null;
            }
        }

        public async Task<bool> ChangeAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, string? notes = null)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning($"Appointment with ID {appointmentId} not found for status change.");
                return false;
            }

            appointment.Status = newStatus;
            if (!string.IsNullOrEmpty(notes))
            {
                appointment.Notes = string.IsNullOrEmpty(appointment.Notes) ? notes : $"{appointment.Notes}\nStatus Change: {notes}";
            }
            appointment.LastModifiedDate = DateTime.UtcNow;

            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Status of Appointment ID {appointment.Id} changed to {newStatus}.");
                return true;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, $"Error changing status for Appointment ID {appointment.Id}.");
                return false;
            }
        }


        public async Task<bool> CancelAppointmentAsync(Guid appointmentId, string cancellationReason, bool cancelledByPatient)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning($"Appointment ID {appointmentId} not found for cancellation.");
                return false;
            }

            appointment.Status = cancelledByPatient ? AppointmentStatus.CancelledByPatient : AppointmentStatus.CancelledByClinic;
            appointment.Notes = string.IsNullOrEmpty(appointment.Notes)
                ? $"Cancelled: {cancellationReason}"
                : $"{appointment.Notes}\nCancelled: {cancellationReason}";
            appointment.LastModifiedDate = DateTime.UtcNow;

            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Appointment ID {appointment.Id} cancelled. Reason: {cancellationReason}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling appointment ID {appointment.Id}.");
                return false;
            }
        }

        private async Task<AppointmentDto> MapAppointmentToDtoAsync(Appointment appointment)
        {
            string doctorName = "N/A";
            if (appointment.DoctorId != null)
            {
                 // If Doctor navigation property is not loaded, load it.
                if (appointment.Doctor == null)
                {
                    appointment.Doctor = await _userManager.FindByIdAsync(appointment.DoctorId);
                }
                if (appointment.Doctor != null)
                {
                    doctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}".Trim();
                    if (string.IsNullOrWhiteSpace(doctorName)) doctorName = appointment.Doctor.UserName ?? "Unknown Doctor";
                }
            }

            string patientName = "N/A";
            string patientCode = "N/A";
            if(appointment.Patient != null) // Patient should be loaded if queried with Include
            {
                patientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}".Trim();
                patientCode = appointment.Patient.PatientCode;
            }
            else if(appointment.PatientId != Guid.Empty) // Fallback if Patient not loaded
            {
                var pat = await _context.Patients.FindAsync(appointment.PatientId);
                if(pat != null)
                {
                     patientName = $"{pat.FirstName} {pat.LastName}".Trim();
                     patientCode = pat.PatientCode;
                }
            }


            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = patientName,
                PatientCode = patientCode,
                DoctorId = appointment.DoctorId,
                DoctorName = doctorName,
                AppointmentDateTime = appointment.AppointmentDateTime,
                DurationMinutes = appointment.DurationMinutes,
                ServiceType = appointment.ServiceType,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Notes = appointment.Notes,
                CreatedDate = appointment.CreatedDate,
                LastModifiedDate = appointment.LastModifiedDate
            };
        }
    }

    // Helper for EF Core 6 DbFunctions if not available directly (usually it is)
    public static class DbFunctions
    {
        public static DateTime AddMinutes(DateTime date, int minutes)
        {
            // This is a stand-in for EF.Functions.AddMinutes or similar if directly translatable
            // In a real EF query, you'd rely on the EF provider's translation.
            // For in-memory or direct C# evaluation, this is fine.
            return date.AddMinutes(minutes);
        }
    }
}
