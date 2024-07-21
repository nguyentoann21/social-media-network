using Microsoft.EntityFrameworkCore;
using network_server.Models;

namespace network_server.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){ }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        /* ***
         * 
         * Unchecked
         * 
         * *** */
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(ur => new {ur.UserId, ur.RoleId});

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            //seed data for role
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = Guid.NewGuid(), RoleName = "Manager" },
                new Role { RoleId = Guid.NewGuid(), RoleName = "Employee" },
                new Role { RoleId = Guid.NewGuid(), RoleName = "User" });
        }
    }
}
