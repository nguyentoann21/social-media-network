
using EllipticCurve;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using network_server.Settings;

namespace network_server.Services.s_user
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        // Constructor to initialize EmailSender with email settings from configuration
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        // Asynchronous method to send an email
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Create a new MimeMessage instance
            var emailMessage = new MimeMessage();
            // Set the sender's email address and display name
            emailMessage.From.Add(new MailboxAddress("Social Media Network System", _emailSettings.FromEmail));
            // Set the recipient's email address
            emailMessage.To.Add(new MailboxAddress(email, email));
            // Set the email subject
            emailMessage.Subject = subject;
            // Set the email body content
            emailMessage.Body = new TextPart("html") { Text = message };

            // Create a new SmtpClient instance
            using var client = new SmtpClient();
            /* ***
             *
             * Connect to the SMTP server using the configured settings
             * The true parameter indicates that SSL should be used
             * Common ports for SMTP are 587 (TLS), 465 (SSL), and 25 (unencrypted)
             *
             *** */
            await client.ConnectAsync(_emailSettings.SmtpServer, int.Parse(_emailSettings.Port), true);
            // Authenticate with the SMTP server using the configured username and password
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            // Send the email message
            await client.SendAsync(emailMessage);
            // Disconnect from the SMTP server and quit the session
            await client.DisconnectAsync(true);
        }
    }
}
