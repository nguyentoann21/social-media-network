namespace network_server.DTOs
{
    public class UserDto
    {
        /* ***
         * 
         * UserId: The unique identifier for the user.
         * Username: The username of the user.
         * AvatarUrl: The URL pointing to the user's profile picture or avatar.
         * FirstName: The user's first name.
         * LastName: The user's last name.
         * EmailAddress: The user's email address.
         * PhoneNumber: The user's phone number.
         * Address: The user's address.
         * Gender: The user's gender.
         * 
         *** */
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
