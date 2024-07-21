
using Microsoft.Extensions.Configuration;
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

        public SmsSender(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        public async Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(number))
            {
                From = new PhoneNumber(_twilioSettings.FromNumber),
                Body = message
            };

            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
