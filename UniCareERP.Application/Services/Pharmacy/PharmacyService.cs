using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Pharmacy;

namespace UniCareERP.Application.Services.Pharmacy
{
    public class PharmacyService : IPharmacyService
    {
        public Task<DrugDto> CreateDrugAsync(DrugDto drugDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<PrescriptionDto> CreatePrescriptionAsync(PrescriptionDto prescriptionDto)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteDrugAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task DeletePrescriptionAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<DrugDto>> GetAllDrugsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<DrugDto> GetDrugByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<PrescriptionDto> GetPrescriptionByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateDrugAsync(int id, DrugDto drugDto)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdatePrescriptionAsync(int id, PrescriptionDto prescriptionDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
