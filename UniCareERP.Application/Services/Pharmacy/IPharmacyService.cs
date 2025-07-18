using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Pharmacy;

namespace UniCareERP.Application.Services.Pharmacy
{
    public interface IPharmacyService
    {
        Task<IEnumerable<DrugDto>> GetAllDrugsAsync();
        Task<DrugDto> GetDrugByIdAsync(int id);
        Task<DrugDto> CreateDrugAsync(DrugDto drugDto);
        Task UpdateDrugAsync(int id, DrugDto drugDto);
        Task DeleteDrugAsync(int id);

        Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<PrescriptionDto> GetPrescriptionByIdAsync(int id);
        Task<PrescriptionDto> CreatePrescriptionAsync(PrescriptionDto prescriptionDto);
        Task UpdatePrescriptionAsync(int id, PrescriptionDto prescriptionDto);
        Task DeletePrescriptionAsync(int id);
    }
}
