using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.Inpatient;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Domain.Entities.Inpatient;

namespace UniCareERP.Application.Services.Inpatient
{
    public class InpatientService : IInpatientService
    {
        private readonly UniCareDbContext _context;

        public InpatientService(UniCareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InpatientDto>> GetAllInpatientsAsync()
        {
            return await _context.Inpatients
                .Include(i => i.Patient)
                .Select(i => new InpatientDto
                {
                    Id = i.Id,
                    PatientId = i.PatientId,
                    PatientName = i.Patient.FirstName + " " + i.Patient.LastName,
                    AdmissionDate = i.AdmissionDate,
                    DischargeDate = i.DischargeDate,
                    RoomNumber = i.RoomNumber,
                    BedNumber = i.BedNumber,
                    AdmissionReason = i.AdmissionReason,
                    DischargeReason = i.DischargeReason
                })
                .ToListAsync();
        }

        public async Task<InpatientDto> GetInpatientByIdAsync(Guid id)
        {
            var inpatient = await _context.Inpatients
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inpatient == null)
                return null;

            return new InpatientDto
            {
                Id = inpatient.Id,
                PatientId = inpatient.PatientId,
                PatientName = inpatient.Patient.FirstName + " " + inpatient.Patient.LastName,
                AdmissionDate = inpatient.AdmissionDate,
                DischargeDate = inpatient.DischargeDate,
                RoomNumber = inpatient.RoomNumber,
                BedNumber = inpatient.BedNumber,
                AdmissionReason = inpatient.AdmissionReason,
                DischargeReason = inpatient.DischargeReason
            };
        }

        public async Task<InpatientDto> CreateInpatientAsync(CreateInpatientDto createInpatientDto)
        {
            var inpatient = new Domain.Entities.Inpatient.Inpatient
            {
                PatientId = createInpatientDto.PatientId,
                AdmissionDate = createInpatientDto.AdmissionDate,
                DischargeDate = createInpatientDto.DischargeDate,
                RoomNumber = createInpatientDto.RoomNumber,
                BedNumber = createInpatientDto.BedNumber,
                AdmissionReason = createInpatientDto.AdmissionReason,
                DischargeReason = createInpatientDto.DischargeReason
            };

            _context.Inpatients.Add(inpatient);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FindAsync(inpatient.PatientId);

            return new InpatientDto
            {
                Id = inpatient.Id,
                PatientId = inpatient.PatientId,
                PatientName = patient.FirstName + " " + patient.LastName,
                AdmissionDate = inpatient.AdmissionDate,
                DischargeDate = inpatient.DischargeDate,
                RoomNumber = inpatient.RoomNumber,
                BedNumber = inpatient.BedNumber,
                AdmissionReason = inpatient.AdmissionReason,
                DischargeReason = inpatient.DischargeReason
            };
        }

        public async Task<bool> UpdateInpatientAsync(UpdateInpatientDto updateInpatientDto)
        {
            var inpatient = await _context.Inpatients.FindAsync(updateInpatientDto.Id);

            if (inpatient == null)
                return false;

            inpatient.PatientId = updateInpatientDto.PatientId;
            inpatient.AdmissionDate = updateInpatientDto.AdmissionDate;
            inpatient.DischargeDate = updateInpatientDto.DischargeDate;
            inpatient.RoomNumber = updateInpatientDto.RoomNumber;
            inpatient.BedNumber = updateInpatientDto.BedNumber;
            inpatient.AdmissionReason = updateInpatientDto.AdmissionReason;
            inpatient.DischargeReason = updateInpatientDto.DischargeReason;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInpatientAsync(Guid id)
        {
            var inpatient = await _context.Inpatients.FindAsync(id);

            if (inpatient == null)
                return false;

            _context.Inpatients.Remove(inpatient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
