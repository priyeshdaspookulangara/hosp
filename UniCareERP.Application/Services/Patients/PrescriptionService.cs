using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Patients
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(UniCareDbContext context, ILogger<PrescriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PrescriptionDto?> CreatePrescriptionAsync(CreatePrescriptionDto createDto, string doctorId)
        {
            // Validate patient and inventory items exist
            if (!await _context.Patients.AnyAsync(p => p.Id == createDto.PatientId))
            {
                _logger.LogWarning($"Attempted to create prescription for non-existent patient ID: {createDto.PatientId}");
                return null;
            }

            var itemIds = createDto.Items.Select(i => i.InventoryItemId).ToList();
            var validItemCount = await _context.InventoryItems.CountAsync(i => itemIds.Contains(i.Id));
            if(validItemCount != itemIds.Count)
            {
                _logger.LogWarning($"Prescription contains non-existent inventory item IDs.");
                return null;
            }

            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                PatientId = createDto.PatientId,
                DoctorId = doctorId,
                PrescriptionDate = DateTime.UtcNow,
                SourceAppointmentId = createDto.SourceAppointmentId,
                Status = PrescriptionStatus.Active,
                Items = createDto.Items.Select(itemDto => new PrescriptionItem
                {
                    Id = Guid.NewGuid(),
                    InventoryItemId = itemDto.InventoryItemId,
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Duration = itemDto.Duration,
                    Notes = itemDto.Notes
                }).ToList()
            };

            try
            {
                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Prescription {prescription.Id} created by doctor {doctorId} for patient {prescription.PatientId}.");
                return await GetPrescriptionByIdAsync(prescription.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription.");
                return null;
            }
        }

        public async Task<PrescriptionDto?> GetPrescriptionByIdAsync(Guid prescriptionId)
        {
            var prescription = await _context.Prescriptions
                                             .Include(p => p.Patient)
                                             .Include(p => p.Doctor)
                                             .Include(p => p.Items)
                                             .ThenInclude(i => i.InventoryItem)
                                             .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            return prescription == null ? null : MapPrescriptionToDto(prescription);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsForPatientAsync(Guid patientId)
        {
            var prescriptions = await _context.Prescriptions
                                              .Where(p => p.PatientId == patientId)
                                              .Include(p => p.Doctor)
                                              .Include(p => p.Items)
                                              .ThenInclude(i => i.InventoryItem)
                                              .OrderByDescending(p => p.PrescriptionDate)
                                              .ToListAsync();

            return prescriptions.Select(MapPrescriptionToDto);
        }

        public async Task<bool> CancelPrescriptionAsync(Guid prescriptionId, string userId)
        {
            var prescription = await _context.Prescriptions.FindAsync(prescriptionId);
            if (prescription == null)
            {
                _logger.LogWarning($"Prescription {prescriptionId} not found for cancellation.");
                return false;
            }

            // Business rule: Only the prescribing doctor can cancel it, and only if it's active.
            if (prescription.DoctorId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to cancel prescription {prescriptionId} not belonging to them.");
                return false;
            }
            if (prescription.Status != PrescriptionStatus.Active)
            {
                _logger.LogWarning($"Attempted to cancel prescription {prescriptionId} which is not in Active state.");
                return false;
            }

            prescription.Status = PrescriptionStatus.Cancelled;
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Prescription {prescriptionId} cancelled by doctor {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling prescription {prescriptionId}.");
                return false;
            }
        }

        private static PrescriptionDto MapPrescriptionToDto(Prescription p)
        {
            return new PrescriptionDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient != null ? $"{p.Patient.FirstName} {p.Patient.LastName}".Trim() : "N/A",
                DoctorName = p.Doctor != null ? $"{p.Doctor.FirstName} {p.Doctor.LastName}".Trim() : "N/A",
                PrescriptionDate = p.PrescriptionDate,
                Status = p.Status,
                Items = p.Items?.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    InventoryItemId = i.InventoryItemId,
                    ItemName = i.InventoryItem?.Name ?? "N/A",
                    Dosage = i.Dosage,
                    Frequency = i.Frequency,
                    Duration = i.Duration,
                    Notes = i.Notes
                }).ToList() ?? new List<PrescriptionItemDto>()
            };
        }
    }
}
