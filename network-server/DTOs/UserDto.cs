namespace network_server.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
    }
}
