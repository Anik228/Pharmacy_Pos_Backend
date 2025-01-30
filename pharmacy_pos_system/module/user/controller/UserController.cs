using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pharmacy_pos_system.module.user.model;
using pharmacy_pos_system.module.user.service;
using System.Net.Http;
using System.Net.Http.Json;

namespace pharmacy_pos_system.module.user.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _loginService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {

                var user = await _loginService.GetUserByEmailAsync(loginDto.Email);

                Console.WriteLine(user);


                if (user == null)
                {
                    Console.WriteLine($"User with email {loginDto.Email} not found.");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

                if (!isPasswordValid)
                {


                    Console.WriteLine($"User with email {loginDto.Email} not found.");
                }

                var token = await _loginService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = "Invalid credential" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("add-user")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                registerDto.Password = hashedPassword;

                await _loginService.AddUserAsync(registerDto);
                return Ok(new { Message = "User added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("user")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _loginService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-user/{id:int}")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _loginService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPut("update-user/{id:int}")]
        [Authorize]
        [Authorize(Roles = "admin,user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateUser(int id, UserDto user)
        {
            if (user == null || id <= 0)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 404,
                    message = "Bad Request"
                });
            }

            bool result = await _loginService.UpdateUserAsync(id, user);

            if (!result)
            {
                return NotFound(new common.ApiErros
                {
                    status = 404,
                    message = "User not found."
                });
            }

            return Ok(new common.ApiErros
            {
                status = 200,
                message = "Successfully Updated"
            });
        }

        [HttpGet("find-a-user/{id}")]
        [Authorize]
        [Authorize(Roles = "admin,user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public Task<User> GetTask(int id) => _loginService.GetUserByIdAsync(id);


        [HttpPut("update-user-password/{id:int}")]
        [Authorize]
        [Authorize(Roles = "admin,user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateUserPassword(int id, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (updatePasswordDto == null || string.IsNullOrEmpty(updatePasswordDto.NewPassword) || id <= 0)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 400,
                    message = "Invalid request."
                });
            }

            var user = await _loginService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new common.ApiErros
                {
                    status = 404,
                    message = "User not found."
                });
            }

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(updatePasswordDto.OldPassword, user.Password);
            if (!isOldPasswordValid)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 400,
                    message = "Old password is incorrect."
                });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.NewPassword);

            bool result = await _loginService.UpdateUserPasswordAsync(id, user.Password);

            if (!result)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 400,
                    message = "Failed to update password."
                });
            }

            return Ok(new common.ApiErros
            {
                status = 200,
                message = "Password updated successfully."
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(googleLoginDto.IdToken))
                {
                    return BadRequest(new { Message = "ID token is required" });
                }

                // Log the first and last few characters of the token for debugging
                var tokenPreview = googleLoginDto.IdToken.Length > 50 
                    ? $"{googleLoginDto.IdToken.Substring(0, 20)}...{googleLoginDto.IdToken.Substring(googleLoginDto.IdToken.Length - 20)}"
                    : googleLoginDto.IdToken;
                Console.WriteLine($"Received Google token: {tokenPreview}");

                var token = await _loginService.LoginWithGoogleAsync(googleLoginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Google login failed: {ex.Message}");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in Google login: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] GoogleCallbackDto callbackDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(callbackDto.Error))
                {
                    return BadRequest(new { Message = "Google authentication failed" });
                }

                if (string.IsNullOrEmpty(callbackDto.Code))
                {
                    return BadRequest(new { Message = "Authorization code is missing" });
                }

                var token = await _loginService.HandleGoogleCallbackAsync(callbackDto.Code);
                
                // You might want to redirect to a frontend URL with the token
                var frontendUrl = _configuration["GoogleAuth:FrontendRedirectUrl"];
                return Redirect($"{frontendUrl}?token={token}");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
