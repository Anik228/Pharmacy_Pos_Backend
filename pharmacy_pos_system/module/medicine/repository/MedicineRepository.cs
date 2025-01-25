using Microsoft.EntityFrameworkCore;
using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.context;

namespace pharmacy_pos_system.module.medicine.repository
{

    public class MedicineRepository : IMedicineRepository
        {
            private readonly DbContextCommon _context;

            public MedicineRepository(DbContextCommon context)
            {
                _context = context;
            }

            public async Task AddAsync(Medicine medicine)
            {
                await _context.Medicines.AddAsync(medicine);
                await _context.SaveChangesAsync();
            }

            public async Task<List<Medicine>> GetAllAsync() =>
                await _context.Medicines.ToListAsync();

            public async Task DeleteMedicineAsync(int id)
            {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
            }
            }

            public async Task UpdateMedicineAsync(Medicine medicine)
            {
            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();
            }

            public async Task<Medicine> GetMedicineByIdAsync(int id) => await _context.Medicines.FindAsync(id);

        public async Task<Medicine> GetMedicineByNameAsync(string name)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.Name == name);
        }


    }
}

