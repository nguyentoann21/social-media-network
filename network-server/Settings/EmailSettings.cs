namespace network_server.Settings
{
    public class EmailSettings
    {
        /* ***
         * 
         * Gets or sets the email address used as the sender's address for outgoing emails.
         * Gets or sets the SMTP server address used for sending emails.
         * Gets or sets the port number used for connecting to the SMTP server.
         * Gets or sets the username used for authenticating with the SMTP server.
         * Gets or sets the password used for authenticating with the SMTP server.
         * 
         *** */
        public string FromEmail { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
