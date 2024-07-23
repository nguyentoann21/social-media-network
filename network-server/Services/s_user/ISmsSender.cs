namespace network_server.Services.s_user
{
    /* ***
     * 
     * ISmsSender interface defines a contract for sending SMS messages.
     * Implementations of this interface should provide the logic for sending an SMS.
     * 
     *** */
    public interface ISmsSender
    {
        /* ***
         * Asynchronously sends an SMS message.
         * 
         * Parameters:
         * - number: The recipient's phone number.
         * - message: The content of the SMS message.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task SendSmsAsync(string number, string message);
    }
}
