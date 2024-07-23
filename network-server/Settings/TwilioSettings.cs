namespace network_server.Settings
{
    public class TwilioSettings
    {
        /* ***
         * 
         * Gets or sets the Account SID for Twilio API.
         * Gets or sets the Auth Token for Twilio API.
         * Gets or sets the phone number that will be used as the sender's number for Twilio messages.
         * 
         *** */
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
    }

}
