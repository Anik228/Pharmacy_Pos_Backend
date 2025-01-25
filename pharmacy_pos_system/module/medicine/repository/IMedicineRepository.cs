using pharmacy_pos_system.module.medicine.model;

namespace pharmacy_pos_system.module.medicine.repository
{

        public interface IMedicineRepository
        {
            Task AddAsync(Medicine medicine);
            Task<List<Medicine>> GetAllAsync();
            Task DeleteMedicineAsync(int id);
            Task<Medicine> GetMedicineByIdAsync(int id);

            Task<Medicine> GetMedicineByNameAsync(string name);
            Task UpdateMedicineAsync(Medicine medicine);

    }
    
}
