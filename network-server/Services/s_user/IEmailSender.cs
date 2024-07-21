namespace network_server.Services.s_user
{
    /* ***
     * 
     * Unchecked
     * 
     * *** */
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
