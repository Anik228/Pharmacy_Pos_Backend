using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pharmacy_pos_system.module.user.model;
using pharmacy_pos_system.module.user.service;


namespace pharmacy_pos_system.module.user.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _loginService;

        public UserController(IUserService loginService)
        {
            _loginService = loginService;
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

            // Verify old password before updating
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(updatePasswordDto.OldPassword, user.Password);
            if (!isOldPasswordValid)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 400,
                    message = "Old password is incorrect."
                });
            }

            // Hash the new password
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

    }
}
