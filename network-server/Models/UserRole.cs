namespace network_server.Models
{
    public class UserRole
    {
        /* *** 
         * 
         * UserId: Foreign key referencing the `User` entity. Represents the user associated with this role.
         * RoleId: Foreign key referencing the `Role` entity. Represents the role assigned to this user.
         * 
         *** */

        // Foreign key to the User entity
        public Guid UserId { get; set; }
        // Navigation property to the User entity
        public User? User { get; set; }
        // Foreign key to the Role entity
        public Guid RoleId { get; set; }
        // Navigation property to the Role entity
        public Role? Role { get; set; }
    }
}
