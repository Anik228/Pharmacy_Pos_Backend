using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.role.model;
using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.module.role.service
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(CreateRoleDto dto);
        Task<List<Role>> GetRoleAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task<Role> GetRoleByRolenameAsync(string username);
        Task<bool> UpdateRoleAsync(RoleDto dto);
        Task<bool> DeleteRole(int roleId);
    }
}
