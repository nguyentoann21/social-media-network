namespace network_server.DTOs
{
    public class UpdateProfileDto
    {
        /* ***
         * 
         * FirstName: The user's first name, to be updated in their profile.
         * LastName: The user's last name, to be updated in their profile.
         * EmailAddress: The user's email address, to be updated in their profile.
         * PhoneNumber: The user's phone number, to be updated in their profile.
         * Address: The user's address, to be updated in their profile.
         * Gender: The user's gender, to be updated in their profile.
         * Avatar: An optional file upload for the user's profile picture (avatar).
         * 
         *** */
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
