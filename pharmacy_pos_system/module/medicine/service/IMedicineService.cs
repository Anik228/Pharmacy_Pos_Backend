using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.module.medicine.service
{

    public interface IMedicineService
        {
            
            Task AddMedicineAsync(AddMedicineDto registerDto);
            Task<List<Medicine>> GetAllMedicineAsync();
           
            Task DeleteMedicineAsync(int id);

            Task<bool> UpdateMedicineAsync(int id, AddMedicineDto medicineDto);

            Task<Medicine> GetMedicineByIdAsync(int id);

           

    }

}
