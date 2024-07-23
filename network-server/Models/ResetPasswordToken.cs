using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class ResetPasswordToken
    {

        /* ***
         * 
         * TokenId: This is the primary key for the ResetPasswordToken entity.
         * UserId: This is a foreign key that references the primary key of the User entity.
         * PasswordResetCode: This is a random code used for password reset, typically a 6-digit code.
         * ExpirationTime: This is the time at which the reset code expires, used to determine if the code is still valid.
         * User: This is a navigation property that represents the relationship with the User entity.
         * 
         *** */
        [Key]
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string PasswordResetCode { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public User? User { get; set; }
    }
}
