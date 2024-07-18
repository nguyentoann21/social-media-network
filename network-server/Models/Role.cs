using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
