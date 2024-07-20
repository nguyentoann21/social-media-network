namespace network_server.DTOs
{
    public class LoginDto
    {
        public string UsernameOrEmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
