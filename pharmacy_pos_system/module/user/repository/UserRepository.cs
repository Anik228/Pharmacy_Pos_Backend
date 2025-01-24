using Microsoft.EntityFrameworkCore;
using pharmacy_pos_system.module.user.model;
using pharmacy_pos_system.context;

namespace pharmacy_pos_system.module.user.repository
{

    public class UserRepository : IUserRepository
        {
            private readonly DbContextCommon _context;

            public UserRepository(DbContextCommon context)
            {
                _context = context;
            }

            public async Task<User> GetByEmailAsync(string email) =>
                await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            public async Task AddAsync(User login)
            {
                await _context.Users.AddAsync(login);
                await _context.SaveChangesAsync();
            }

            public async Task<List<User>> GetAllAsync() =>
                await _context.Users.ToListAsync();

            public async Task DeleteUserAsync(int id)
            {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            }

            public async Task UpdateUserAsync(User user)
            {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            }

            public async Task<User> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);

    }
    }

