using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.module.user.repository
{

        public interface IUserRepository
        {
            Task<User> GetByEmailAsync(string email);
            Task AddAsync(User login);
            Task<List<User>> GetAllAsync();
            Task DeleteUserAsync(int id);
            Task<User> GetUserByIdAsync(int id);
            Task UpdateUserAsync(User user);

            

    }
    
}
