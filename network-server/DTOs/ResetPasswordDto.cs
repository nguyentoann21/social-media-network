namespace network_server.DTOs
{
    public class ResetPasswordDto
    {
        /* ***
         * 
         * Unchecked
         * 
         * *** */
        public string EmailAddressOrPhoneNumber { get; set; } = string.Empty;
        public string PasswordToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
