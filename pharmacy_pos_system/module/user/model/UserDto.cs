namespace pharmacy_pos_system.module.user.model
{
   
        public class UserDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Phone { get; set; }
             public string Password { get; set; }
        }

    public class UpdatePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }


    public class LoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisterDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Phone { get; set; }
        }
    
}
