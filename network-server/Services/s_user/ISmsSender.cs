namespace network_server.Services.s_user
{
    /* ***
     * 
     * Unchecked
     * 
     * *** */
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
