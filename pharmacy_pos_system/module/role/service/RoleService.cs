using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using pharmacy_pos_system.common;
using pharmacy_pos_system.context;
using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.medicine.repository;
using pharmacy_pos_system.module.medicine.service;
using pharmacy_pos_system.module.role.model;
using pharmacy_pos_system.module.user.model;
using System.Security.Cryptography;

namespace pharmacy_pos_system.module.role.service
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IPharmacyRepository<Role> _pharmacyRepository;
        public RoleService(IPharmacyRepository<Role> pharmacyRepository, IMapper mapper)
        {
            _pharmacyRepository = pharmacyRepository;
            _mapper = mapper;
        }
        public async Task<bool> CreateRoleAsync(CreateRoleDto dto)
        {
           
            ArgumentNullException.ThrowIfNull(dto, $"the argument {nameof(dto)} is null");

            var existingRole = await _pharmacyRepository.GetAsync(u => u.Name.Equals(dto.Name));

            if (existingRole != null)
            {
                throw new Exception("The Role name already taken");
            }

            Role role = _mapper.Map<Role>(dto);


            role.IsDeleted = false;
          
            await _pharmacyRepository.CreateAsync(role);

            return true;
        }

        public async Task<List<Role>> GetRoleAsync()
        {
            var role = await _pharmacyRepository.GetAllByFilterAsync(u => !u.IsDeleted);

            return _mapper.Map<List<Role>>(role);
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _pharmacyRepository.GetAsync(u => !u.IsDeleted && u.Id == id);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<Role> GetRoleByRolenameAsync(string rolename)
        {
            var user = await _pharmacyRepository.GetAsync(u => !u.IsDeleted && u.Name.Equals(rolename));

            // return _mapper.Map<RoleDto>(user);

            return user;
        }

        public async Task<bool> UpdateRoleAsync(RoleDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));

            var existingUser = await _pharmacyRepository.GetAsync(u => !u.IsDeleted && u.Id == dto.Id, true);
            if (existingUser == null)
            {
                throw new Exception($"Role not found with the Id: {dto.Id}");
            }

            var roleToUpdate = _mapper.Map<Role>(dto);
            roleToUpdate.UpdatedAt = DateTime.Now;
           

            await _pharmacyRepository.UpdateAsync(roleToUpdate);

            return true;
        }
        public async Task<bool> DeleteRole(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException(nameof(roleId));

            var existingUser = await _pharmacyRepository.GetAsync(u => !u.IsDeleted && u.Id == roleId, true);
            if (existingUser == null)
            {
                throw new Exception($"User not found with the Id: {roleId}");
            }

            existingUser.IsDeleted = true;

            await _pharmacyRepository.UpdateAsync(existingUser);

            return true;
        }
       

    }
}
