using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class User
    {
        /* ***
         * 
         * UserId: The primary key for the User entity. It uniquely identifies each user.
         * Username: The user's username, which should be unique.
         * AvatarUrl: The URL of the user's avatar image.
         * FirstName: The user's first name.
         * LastName: The user's last name.
         * Password: The hashed password for the user.
         * EmailAddress: The user's email address, validated as an email format.
         * PhoneNumber: The user's phone number.
         * Address: The user's address.
         * Gender: The user's gender.
         * UserRoles: A collection of `UserRole` entities that represent the roles assigned to this user.
         * ResetPasswordTokens: A collection of `ResetPasswordToken` entities associated with this user for password reset purposes.
         * 
         *** */
        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = new HashSet<ResetPasswordToken>();
    }
}
