
using Microsoft.Extensions.Options;
using network_server.Settings;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace network_server.Services.s_user
{
    public class SmsSender : ISmsSender
    {

        private readonly TwilioSettings _twilioSettings;

        /* ***
         * Constructor that initializes the SmsSender with Twilio settings.
         * 
         * Parameters:
         * - twilioSettings: Provides Twilio configuration settings through IOptions.
         * 
         *** */
        public SmsSender(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        /* ***
         * Sends an SMS message using the Twilio API.
         * 
         * Parameters:
         * - number: The recipient's phone number in E.164 format (e.g., +1234567890).
         * - message: The content of the SMS message to be sent.
         * 
         * Returns:
         * - A Task representing the asynchronous operation of sending the SMS.
         * 
         *** */
        public async Task SendSmsAsync(string number, string message)
        {
            // Initialize the Twilio client with Account SID and Auth Token
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

            // Create message options with the recipient's phone number, sender's phone number, and message body
            var messageOptions = new CreateMessageOptions(new PhoneNumber(number))
            {
                From = new PhoneNumber(_twilioSettings.FromNumber),
                Body = message
            };

            // Send the SMS message asynchronously and await completion
            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
