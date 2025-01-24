using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.module.user.service
{

    public interface IUserService
        {
            Task<string> LoginAsync(LoginDto loginDto);
            Task AddUserAsync(RegisterDto registerDto);
            Task<List<UserDto>> GetAllUsersAsync();
            Task<UserDto> GetUserByEmailAsync(string email);

            Task DeleteUserAsync(int id);

            Task<bool> UpdateUserAsync(int id, UserDto userDto);

            Task<User> GetUserByIdAsync(int id);

            Task<bool> UpdateUserPasswordAsync(int id, string newPassword);

    }

}
