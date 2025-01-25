using pharmacy_pos_system.module.user.model;
using System.Security.Claims;
using System.Text;
using pharmacy_pos_system.module.user.repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using pharmacy_pos_system.context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.medicine.repository;

namespace pharmacy_pos_system.module.medicine.service
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly DbContextCommon _dbContext;

        public MedicineService(IMedicineRepository repository, IConfiguration configuration, DbContextCommon dbContext)
        {
            _repository = repository;
            _configuration = configuration;
            _dbContext = dbContext;
        }
      
        public async Task AddMedicineAsync(AddMedicineDto registerDto)
        {
            var medicine = new Medicine
            {
                Name = registerDto.Name,
                Brand= registerDto.Brand,
                Image = registerDto.Image,
                Description = registerDto.Description,
                Price = registerDto.Price,
                Quantity = registerDto.Quantity,
                CreatedAt = registerDto.CreatedAt,
                Expiry_date= registerDto.Expiry_date
            };

            await _repository.AddAsync(medicine);
        }

        public async Task<List<Medicine>> GetAllMedicineAsync()
        {
            var medicine = await _repository.GetAllAsync();
            return medicine.Select(u => new Medicine
            {
                Id= u.Id,
                Name = u.Name,
                Brand = u.Brand,
                Image = u.Image,
                Description = u.Description,
                Price = u.Price,
                Quantity = u.Quantity,
                CreatedAt = u.CreatedAt,
                Expiry_date = u.Expiry_date

            }).ToList();
        }

       
        public Task DeleteMedicineAsync(int id) => _repository.DeleteMedicineAsync(id);

        public async Task<bool> UpdateMedicineAsync(int id, AddMedicineDto medicineDto)
        {
            var existingMedicine = await _repository.GetMedicineByIdAsync(id);

            if (existingMedicine == null)
            {
                return false;
            }

            existingMedicine.Name = medicineDto.Name;
            existingMedicine.Brand = medicineDto.Brand;
            existingMedicine.Description = medicineDto.Description;
            existingMedicine.Image= medicineDto.Image;
            existingMedicine.Price = medicineDto.Price;
            existingMedicine.Quantity = medicineDto.Quantity;
            existingMedicine.CreatedAt = medicineDto.CreatedAt;
            existingMedicine.Expiry_date = medicineDto.Expiry_date;
           
            await _repository.UpdateMedicineAsync(existingMedicine);
            return true;
        }
      
        public Task<Medicine> GetMedicineByIdAsync(int id) => _repository.GetMedicineByIdAsync(id);

        public Task<Medicine> GetMedicineByNameAsync(string name) => _repository.GetMedicineByNameAsync(name);

    }
}
