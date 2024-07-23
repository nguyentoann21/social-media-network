namespace network_server.DTOs
{
    public class UpdatePasswordDto
    {
        /* ***
         * 
         * CurrentPassword: The user's current password, used to verify their identity before updating to a new password.
         * NewPassword: The new password that the user wants to set for their account.
         * 
         *** */
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
