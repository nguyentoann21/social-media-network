
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using network_server.Settings;

namespace network_server.Services.s_user
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(
                new MailboxAddress("Social-Media-Network-Arthur", _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };
            
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, int.Parse(_emailSettings.Port), true);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
