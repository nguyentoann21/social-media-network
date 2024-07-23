namespace network_server.DTOs
{
    public class RegisterDto
    {
        /* ***
         * 
         * Username: The username for the new user. Must be a unique identifier.
         * Avatar: An optional file for the user's profile picture.
         * FirstName: The user's first name.
         * LastName: The user's last name.
         * Password: The user's password, which should be securely hashed.
         * EmailAddress: The user's email address. Must be in a valid email format.
         * PhoneNumber: The user's phone number. Optional.
         * Address: The user's address. Optional.
         * Gender: The user's gender. Optional.
         * RoleName: The role to assign to the new user. Defaults to "User" if not specified.
         * 
         *** */
        public string Username { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
