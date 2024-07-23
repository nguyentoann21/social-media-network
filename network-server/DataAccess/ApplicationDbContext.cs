using Microsoft.EntityFrameworkCore;
using network_server.Models;

namespace network_server.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base class constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){ }

        // DbSet properties for the application's entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

        // Configures the model using the ModelBuilder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuring the composite key for the UserRole entity
            modelBuilder.Entity<UserRole>().HasKey(ur => new {ur.UserId, ur.RoleId});

            // Configuring the relationship between UserRole and User
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // Configuring the relationship between UserRole and Role
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            //seed data for the Role entity
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = Guid.NewGuid(), RoleName = "Manager" },
                new Role { RoleId = Guid.NewGuid(), RoleName = "Employee" },
                new Role { RoleId = Guid.NewGuid(), RoleName = "User" });
        }
    }
}
