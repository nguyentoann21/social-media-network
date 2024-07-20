using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using network_server.DataAccess;
using network_server.DTOs;
using network_server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace network_server.Services.s_user
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task AssignRole(User user, string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);

            if (role == null)
            {
                throw new ApplicationException("Role not found");
            }

            var existingUser = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser == null)
            {
                throw new ApplicationException("User not found");
            }

            if (existingUser.UserRoles != null && existingUser.UserRoles.Any())
            {
                _context.UserRoles.RemoveRange(existingUser.UserRoles);
            }

            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        private async Task<string> AvatarStorageAsync(IFormFile avatar)
        {
            if (avatar == null) return "default-avatar.png";

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatar.FileName;
            var storageFolder = Path.Combine("wwwroot", "avt");
            Directory.CreateDirectory(storageFolder);
            var avatarFilePath = Path.Combine(storageFolder, uniqueFileName);

            using (var fileStream = new FileStream(avatarFilePath, FileMode.Create))
            {
                await avatar.CopyToAsync(fileStream);
            }

            return Path.Combine(string.Empty, uniqueFileName);
        }

        private async Task<User> FindUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }
            return user;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            var user = await FindUserByIdAsync(userId);
            
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            return new UserProfileDto
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender
            };
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.UserRoles == null || !user.UserRoles.Any())
            {
                throw new ApplicationException("User role not found");
            }

            var roleName = user?.UserRoles.FirstOrDefault()?.Role?.RoleName;
            
            if (roleName == null)
            {
                throw new ApplicationException("User role not found");
            }

            return roleName;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmailAddress || u.EmailAddress == loginDto.UsernameOrEmailAddress);

            if (user == null) 
            {
                throw new ApplicationException("Invalid credentials");
            }

            var passwordHash = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!passwordHash)
            {
                throw new ApplicationException("Invalid credentials");
            }

            var role = user.UserRoles.FirstOrDefault()?.Role?.RoleName;

            if (role == null)
            {
                throw new ApplicationException("User role not found");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("EmailAddress", user.EmailAddress),
                new Claim("PhoneNumber", user.PhoneNumber ?? ""), // make sure PhoneNumber not null
                new Claim("Address", user.Address ?? ""), // make sure Address not null
                new Claim("Gender", user.Gender ?? ""), // make sure Gender not null
                new Claim("AvatarUrl", user.AvatarUrl ?? ""), // make sure AvatarUrl not null
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = 
                new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto, string roleName = "User")
        {
            var user = await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.EmailAddress == registerDto.EmailAddress);
            
            if(user)
            {
                throw new ApplicationException("Username/EmailAddress already exist in another account");
            }

            //save file upload
            var avatarUrl = registerDto.Avatar != null ? 
                await AvatarStorageAsync(registerDto.Avatar) : "default-avatar.png"; ;

            var registerUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = registerDto.Username,
                AvatarUrl = avatarUrl,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                EmailAddress = registerDto.EmailAddress,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                Gender = registerDto.Gender
            };

            _context.Users.Add(registerUser);
            await _context.SaveChangesAsync();

            // Assign role after saving user
            await AssignRole(registerUser, roleName);

            return new UserDto
            {
                UserId = registerUser.UserId,
                Username = registerUser.Username,
                AvatarUrl = avatarUrl,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                EmailAddress = registerUser.EmailAddress,
                PhoneNumber = registerUser.PhoneNumber,
                Address = registerUser.Address,
                Gender = registerUser.Gender
            };
        }

        public Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
        {
            return RegisterAsync(registerDto, "User");
        }

        public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePassword)
        {
            var user = await FindUserByIdAsync(userId);

            var passwordHash = BCrypt.Net.BCrypt.Verify(updatePassword.CurrentPassword, user.Password);
            
            if (!passwordHash)
            {
                throw new ApplicationException("Current password is incorrect");
            }

            var checkPassword = updatePassword.CurrentPassword.Equals(updatePassword.NewPassword);

            if (checkPassword)
            {
                throw new ApplicationException("Current password and new password must be different");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(updatePassword.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto updateProfile)
        {
            var user = await FindUserByIdAsync(userId);

            user.FirstName = updateProfile.FirstName;
            user.LastName = updateProfile.LastName;
            user.EmailAddress = updateProfile.EmailAddress;
            user.PhoneNumber = updateProfile.PhoneNumber;
            user.Address = updateProfile.Address;
            user.Gender = updateProfile.Gender;

            //save file upload
            var avatarUrl = updateProfile.Avatar != null ? 
                await AvatarStorageAsync(updateProfile.Avatar) : "default-avatar.png";
            if (avatarUrl != null)
            {
                user.AvatarUrl = avatarUrl;
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
