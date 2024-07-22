using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class ResetPasswordToken
    {

        /* ***
         * 
         * Unchecked
         * 
         * *** */
        [Key]
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string PasswordToken { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public User? User { get; set; }
    }
}
