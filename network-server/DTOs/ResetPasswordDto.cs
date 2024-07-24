namespace network_server.DTOs
{
    public class ResetPasswordDto
    {
        /* ***
         * 
         * EmailAddressOrPhoneNumber: The email address or phone number associated with the user's account,
         *                              used to identify the user requesting a password reset.
         * PasswordToken: The token received by the user to verify the reset request.
         * NewPassword: The new password to be set for the user's account.
         * 
         *** */
        public string EmailAddress { get; set; } = string.Empty;
        public string PasswordToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
