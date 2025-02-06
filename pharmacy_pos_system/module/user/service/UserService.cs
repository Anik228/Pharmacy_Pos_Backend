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
using Google.Apis.Auth;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Google.Apis.Util;
using System.Collections.Concurrent;

namespace pharmacy_pos_system.module.user.service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly DbContextCommon _dbContext;

        // Update the blacklist to use a concurrent set for thread safety
        private static readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public UserService(IUserRepository repository, IConfiguration configuration, DbContextCommon dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
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
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
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
                Phone = registerDto.Phone,
                IsActive=true
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
                Password = user.Password,
                IsActive=user.IsActive
            };
        }

        public Task DeleteUserAsync(int id) => _repository.DeleteUserAsync(id);

        public async Task<bool> UpdateUserAsync(int id, UserDto userDto,string role)
        {
            var existingUser = await _repository.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                return false;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            if (role == "admin")
            {

                existingUser.Name = userDto.Name;
                existingUser.Email = userDto.Email;
                existingUser.Phone = userDto.Phone;
                existingUser.Password = hashedPassword;
                existingUser.Role = userDto.Role;
                existingUser.IsActive = userDto.IsActive;
            }

            else {

                existingUser.Name = userDto.Name;
                existingUser.Email = userDto.Email;
                existingUser.Phone = userDto.Phone;
                existingUser.Password = hashedPassword;
                existingUser.IsActive = userDto.IsActive;
                //existingUser.Role = userDto.Role;

            }

          
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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> LoginWithGoogleAsync(GoogleLoginDto googleLoginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(googleLoginDto.IdToken))
                {
                    throw new UnauthorizedAccessException("ID token is missing or invalid");
                }

                // Verify the Google ID token
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["GoogleAuth:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, settings);

                if (payload == null)
                {
                    throw new UnauthorizedAccessException("Invalid Google token");
                }

                // Check if user exists in database
                var user = await _repository.GetByEmailAsync(payload.Email);

                if (user == null)
                {
                    // Create new user if doesn't exist
                    user = new User
                    {
                        Email = payload.Email,
                        Name = payload.Name,
                        Role = "user",
                        IsActive = true,
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()) // Generate a random password
                    };

                    await _repository.AddAsync(user);
                }

                // Generate JWT token using the helper method
                return GenerateJwtToken(user);
            }
            catch (InvalidJwtException ex)
            {
                throw new UnauthorizedAccessException($"Google authentication failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Authentication failed: {ex.Message}");
            }
        }

        public async Task<string> HandleGoogleCallbackAsync(string code)
        {
            try
            {
                var clientId = _configuration["GoogleAuth:ClientId"];
                var clientSecret = _configuration["GoogleAuth:ClientSecret"];
                var redirectUri = _configuration["GoogleAuth:RedirectUri"];

                // Exchange authorization code for tokens
                using var httpClient = new HttpClient();
                var tokenResponse = await httpClient.PostAsync(
                    "https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"code", code},
                        {"client_id", clientId},
                        {"client_secret", clientSecret},
                        {"redirect_uri", redirectUri},
                        {"grant_type", "authorization_code"}
                    })
                );

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to exchange authorization code for tokens");
                }

                var tokenData = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
                var idToken = tokenData.GetProperty("id_token").GetString();

                // Verify the ID token
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check if user exists in database
                var user = await _repository.GetByEmailAsync(payload.Email);

                if (user == null)
                {
                    // Create new user if doesn't exist
                    user = new User
                    {
                        Email = payload.Email,
                        Name = payload.Name,
                        Role = "user",
                        IsActive = true
                    };

                    await _repository.AddAsync(user);
                }

                // Use the helper method here
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Google callback processing failed: {ex.Message}");
            }
        }

        public async Task<User> LogoutAsync(int userId)
        {
            try
            {
                // Get the current token
                var token = await GetCurrentToken("Authorization");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No token found");
                }

                // Add token to blacklist with expiration time
                _blacklistedTokens.TryAdd(token, DateTime.UtcNow.AddHours(1));

                // Clean up expired tokens
                CleanupExpiredTokens();

                // Get user details
                var user = await _repository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Logout failed: " + ex.Message);
            }
        }

        public async Task<bool> IsTokenBlacklisted(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out DateTime expirationTime))
            {
                if (DateTime.UtcNow > expirationTime)
                {
                    // Token has expired, remove it from blacklist
                    _blacklistedTokens.TryRemove(token, out _);
                    return false;
                }
                return true;
            }
            return false;
        }

        private void CleanupExpiredTokens()
        {
            var expiredTokens = _blacklistedTokens
                .Where(kvp => DateTime.UtcNow > kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                _blacklistedTokens.TryRemove(token, out _);
            }
        }

        public async Task<string> GetCurrentToken(string requestHeader)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers[requestHeader].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                return await Task.FromResult(authorizationHeader.Substring(7));
            }
            return await Task.FromResult(string.Empty);
        }
    }
}
