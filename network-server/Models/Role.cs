using System.ComponentModel.DataAnnotations;

namespace network_server.Models
{
    public class Role
    {
        /* ***
         * 
         * RoleId: The primary key for the Role entity. It uniquely identifies each role.
         * RoleName: A required string that represents the name of the role. It cannot be null.
         * UserRoles: A collection of `UserRole` entities that are associated with this role.
         * 
         *** */
        [Key]
        public Guid RoleId { get; set; }
        [Required]
        public string RoleName { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
