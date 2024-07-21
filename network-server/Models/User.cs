using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class User
    {
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


        /* ***
         * 
         * Unchecked
         * 
         * *** */
        public ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = new HashSet<ResetPasswordToken>();
    }
}
