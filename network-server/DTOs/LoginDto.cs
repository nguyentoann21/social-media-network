namespace network_server.DTOs
{
    public class LoginDto
    {
        /* ***
         * 
         * UsernameOrEmailAddress: This property allows the user to log in using either their username or email address.
         * Password: This property holds the user's password.
         *
         *** */
        public string UsernameOrEmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
