namespace network_server.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public IFormFile Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string RoleName { get; set; }
    }
}
