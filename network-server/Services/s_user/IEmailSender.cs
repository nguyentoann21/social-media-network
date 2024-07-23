namespace network_server.Services.s_user
{
    /* ***
     * 
     * IEmailSender interface defines a contract for sending emails.
     * Implementations of this interface should provide the logic for sending an email.
     * 
     *** */
    public interface IEmailSender
    {
        /* ***
         * Asynchronously sends an email.
         * 
         * Parameters:
         * - email: The recipient's email address.
         * - subject: The subject of the email.
         * - message: The body content of the email.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task SendEmailAsync(string email, string subject, string message);
    }
}
