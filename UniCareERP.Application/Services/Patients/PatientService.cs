using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Infrastructure.Data; // Assuming UniCareDbContext is here

namespace UniCareERP.Application.Services.Patients
{
    public class PatientService : IPatientService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<PatientService> _logger;
        private const string PatientCodePrefix = "P";

        public PatientService(UniCareDbContext context, ILogger<PatientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateNextPatientCodeAsync()
        {
            // This is a simple way to generate a code.
            // For high concurrency, a database sequence or a dedicated service might be better.
            var lastPatient = await _context.Patients
                                      .OrderByDescending(p => p.PatientCode)
                                      .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastPatient != null && lastPatient.PatientCode.StartsWith(PatientCodePrefix))
            {
                string lastNumberStr = lastPatient.PatientCode.Substring(PatientCodePrefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"{PatientCodePrefix}{nextNumber:D5}"; // e.g., P00001
        }

        public async Task<PatientDto?> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            try
            {
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    PatientCode = await GenerateNextPatientCodeAsync(),
                    FirstName = createPatientDto.FirstName,
                    MiddleName = createPatientDto.MiddleName,
                    LastName = createPatientDto.LastName,
                    DateOfBirth = createPatientDto.DateOfBirth,
                    Gender = createPatientDto.Gender,
                    MaritalStatus = createPatientDto.MaritalStatus,
                    Nationality = createPatientDto.Nationality,
                    PreferredLanguage = createPatientDto.PreferredLanguage,
                    PhoneNumber = createPatientDto.PhoneNumber,
                    Email = createPatientDto.Email,
                    Address = createPatientDto.Address,
                    EmergencyContactName = createPatientDto.EmergencyContactName,
                    EmergencyContactRelationship = createPatientDto.EmergencyContactRelationship,
                    EmergencyContactPhone = createPatientDto.EmergencyContactPhone,
                    BloodGroup = createPatientDto.BloodGroup,
                    Allergies = createPatientDto.Allergies,
                    InsuranceProvider = createPatientDto.InsuranceProvider,
                    InsurancePolicyNumber = createPatientDto.InsurancePolicyNumber,
                    GeneralNotes = createPatientDto.GeneralNotes,
                    RegistrationDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Patient created with ID: {patient.Id} and Code: {patient.PatientCode}");

                return MapPatientToDto(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating patient.");
                return null;
            }
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            return patient == null ? null : MapPatientToDto(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            return await _context.Patients
                                 .OrderBy(p => p.LastName)
                                 .ThenBy(p => p.FirstName)
                                 .Select(p => MapPatientToDto(p))
                                 .ToListAsync();
        }

        public async Task<bool> UpdatePatientAsync(UpdatePatientDto updatePatientDto)
        {
            var patient = await _context.Patients.FindAsync(updatePatientDto.Id);
            if (patient == null)
            {
                _logger.LogWarning($"Patient with ID: {updatePatientDto.Id} not found for update.");
                return false;
            }

            // Update properties
            patient.FirstName = updatePatientDto.FirstName;
            patient.MiddleName = updatePatientDto.MiddleName;
            patient.LastName = updatePatientDto.LastName;
            patient.DateOfBirth = updatePatientDto.DateOfBirth;
            patient.Gender = updatePatientDto.Gender;
            patient.MaritalStatus = updatePatientDto.MaritalStatus;
            patient.Nationality = updatePatientDto.Nationality;
            patient.PreferredLanguage = updatePatientDto.PreferredLanguage;
            patient.PhoneNumber = updatePatientDto.PhoneNumber;
            patient.Email = updatePatientDto.Email;
            patient.Address = updatePatientDto.Address;
            patient.EmergencyContactName = updatePatientDto.EmergencyContactName;
            patient.EmergencyContactRelationship = updatePatientDto.EmergencyContactRelationship;
            patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;
            patient.BloodGroup = updatePatientDto.BloodGroup;
            patient.Allergies = updatePatientDto.Allergies;
            patient.InsuranceProvider = updatePatientDto.InsuranceProvider;
            patient.InsurancePolicyNumber = updatePatientDto.InsurancePolicyNumber;
            patient.GeneralNotes = updatePatientDto.GeneralNotes;
            patient.LastModifiedDate = DateTime.UtcNow;

            try
            {
                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Patient with ID: {patient.Id} updated successfully.");
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency error while updating patient with ID: {patient.Id}.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating patient with ID: {patient.Id}.");
                return false;
            }
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                _logger.LogWarning($"Patient with ID: {id} not found for deletion.");
                return false;
            }

            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Patient with ID: {id} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                // Consider dependencies before deleting (e.g., appointments, medical records)
                // For now, direct delete. Later, might need soft delete or checks.
                _logger.LogError(ex, $"Error occurred while deleting patient with ID: {id}.");
                return false;
            }
        }

        // Manual Mapper from Entity to DTO
        private static PatientDto MapPatientToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                PatientCode = patient.PatientCode,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                MaritalStatus = patient.MaritalStatus,
                Nationality = patient.Nationality,
                PreferredLanguage = patient.PreferredLanguage,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactRelationship = patient.EmergencyContactRelationship,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                BloodGroup = patient.BloodGroup,
                Allergies = patient.Allergies,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber,
                GeneralNotes = patient.GeneralNotes,
                RegistrationDate = patient.RegistrationDate,
                LastModifiedDate = patient.LastModifiedDate
                // Age and FullName are calculated in the DTO itself
            };
        }
    }
}
