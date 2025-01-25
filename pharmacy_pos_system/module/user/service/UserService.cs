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

namespace pharmacy_pos_system.module.user.service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly DbContextCommon _dbContext;

        public UserService(IUserRepository repository, IConfiguration configuration, DbContextCommon dbContext)
        {
            _repository = repository;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _repository.GetByEmailAsync(loginDto.Email);

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (user == null || !isPasswordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task AddUserAsync(RegisterDto registerDto)
        {
            var login = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = registerDto.Role,
                Phone = registerDto.Phone
            };

            await _repository.AddAsync(login);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                Phone = u.Phone
            }).ToList();
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null)
                return null;

            return new UserDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Password=user.Password
            };
        }

        public Task DeleteUserAsync(int id) => _repository.DeleteUserAsync(id);

        public async Task<bool> UpdateUserAsync(int id, UserDto userDto)
        {
            var existingUser = await _repository.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                return false;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            existingUser.Name = userDto.Name;
            existingUser.Email = userDto.Email;
            existingUser.Password = hashedPassword;
            existingUser.Role = userDto.Role;

            await _repository.UpdateUserAsync(existingUser);
            return true;
        }

        
        public Task<User> GetUserByIdAsync(int id) => _repository.GetUserByIdAsync(id);

        public async Task<bool> UpdateUserPasswordAsync(int id, string newPassword)
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                return false; 
            }

            user.Password = newPassword; 

            await _repository.UpdateUserAsync(user);

            return true;
        }





    }
}
