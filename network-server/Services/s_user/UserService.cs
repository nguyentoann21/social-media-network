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
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        /* ***
         * Constructor to initialize the UserService with dependencies.
         * 
         * Parameters:
         * - context: Database context for accessing user-related data.
         * - configuration: Configuration settings for JWT and other services.
         * - emailSender: Service for sending emails.
         * - smsSender: Service for sending SMS messages.
         * 
         *** */
        public UserService(ApplicationDbContext context, IConfiguration configuration, IEmailSender emailSender, ISmsSender smsSender)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        /* ***
         * Assigns a role to a user and removes existing roles.
         * 
         * Parameters:
         * - user: The user to whom the role will be assigned.
         * - roleName: The name of the role to assign.
         * 
         * Throws:
         * - ApplicationException if the role or user is not found.
         * 
         *** */
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

            // Remove existing roles
            if (existingUser.UserRoles != null && existingUser.UserRoles.Any())
            {
                _context.UserRoles.RemoveRange(existingUser.UserRoles);
            }

            // Add new role
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }
        /* ***
         * Saves the avatar file and returns the URL of the saved avatar.
         * 
         * Parameters:
         * - avatar: The file to be saved.
         * 
         * Returns:
         * - The URL of the saved avatar.
         * 
         *** */
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
        /* ***
         * Finds a user by their ID.
         * 
         * Parameters:
         * - userId: The ID of the user to find.
         * 
         * Returns:
         * - The user with the specified ID.
         * 
         * Throws:
         * - ApplicationException if the user is not found.
         * 
         *** */
        private async Task<User> FindUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }
            return user;
        }
        /* ***
         * Retrieves the profile information of a user.
         * 
         * Parameters:
         * - userId: The ID of the user whose profile is to be retrieved.
         * 
         * Returns:
         * - UserProfileDto containing user profile information.
         * 
         * Throws:
         * - ApplicationException if the user is not found.
         * 
         *** */
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
        /* ***
         * Retrieves the role of a user.
         * 
         * Parameters:
         * - userId: The ID of the user whose role is to be retrieved.
         * 
         * Returns:
         * - The name of the role assigned to the user.
         * 
         * Throws:
         * - ApplicationException if the user or role is not found.
         * 
         *** */
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
        /* ***
         * Authenticates a user and generates a JWT token.
         * 
         * Parameters:
         * - loginDto: Contains login credentials.
         * 
         * Returns:
         * - A JWT token for the authenticated user.
         * 
         * Throws:
         * - ApplicationException if the user is not found or credentials are invalid.
         * 
         *** */
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmailAddress || u.EmailAddress == loginDto.UsernameOrEmailAddress);

            if (user == null) 
            {
                throw new ApplicationException("Username/EmailAddress or Password is incorrect");
            }

            var passwordHash = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!passwordHash)
            {
                throw new ApplicationException("Username/EmailAddress or Password is incorrect");
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
                new Claim("PhoneNumber", user.PhoneNumber ?? ""), // Ensure PhoneNumber is not null
                new Claim("Address", user.Address ?? ""), // Ensure Address is not null
                new Claim("Gender", user.Gender ?? ""), // Ensure Gender is not null
                new Claim("AvatarUrl", user.AvatarUrl ?? ""), // Enure AvatarUrl is not null
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
        /* ***
         * Registers a new user and assigns a role.
         * 
         * Parameters:
         * - registerDto: Contains user registration information.
         * - roleName: The role to assign to the user.
         * 
         * Returns:
         * - UserDto containing the registered user information.
         * 
         * Throws:
         * - ApplicationException if the username or email address already exists.
         * 
         *** */
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
        /* ***
         * Registers a new user with the default role "User".
         * 
         * Parameters:
         * - registerDto: Contains user registration information.
         * 
         * Returns:
         * - UserDto containing the registered user information.
         * 
         *** */
        public Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
        {
            return RegisterAsync(registerDto, "User");
        }
        /* ***
         * Updates the user's password.
         * 
         * Parameters:
         * - userId: The ID of the user whose password is to be updated.
         * - updatePassword: Contains current and new passwords.
         * 
         * Throws:
         * - ApplicationException if the current password is incorrect or new password is the same as current password.
         * 
         *** */
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
        /* ***
         * Updates the user's profile information.
         * 
         * Parameters:
         * - userId: The ID of the user whose profile is to be updated.
         * - updateProfile: Contains new profile information.
         * 
         * Throws:
         * - ApplicationException if the user is not found.
         * 
         *** */
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
        /* ***
         * Generates a password reset token for a user.
         * 
         * Parameters:
         * - user: The user for whom the reset token is generated.
         * 
         * Returns:
         * - The generated password reset token.
         * 
         *** */
        private async Task<string> GenerateResetPasswordTokenAsync(User user)
        {
            // Check if there is an existing valid token code for the user
            var existingResetCode = await _context.ResetPasswordTokens
        .FirstOrDefaultAsync(t => t.UserId == user.UserId && t.ExpirationTime > DateTime.UtcNow);

            if (existingResetCode != null)
            {
                // If there is a valid token, return it and edit the expiration time 5 minutes
                existingResetCode.ExpirationTime = DateTime.UtcNow.AddMinutes(5);
                _context.Entry(existingResetCode).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return existingResetCode.PasswordResetCode;
            }


            // Otherwise, generate a new token that means the auth code for the reset password.
            var token = new ResetPasswordToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                PasswordResetCode = new Random().Next(100000, 999999).ToString(),
                ExpirationTime = DateTime.UtcNow.AddMinutes(5)
            };

            _context.ResetPasswordTokens.Add(token);
            await _context.SaveChangesAsync();

            return token.PasswordResetCode;
        }
        /* ***
         * Requests a password reset by sending a reset code via email or SMS.
         * 
         * Parameters:
         * - emailAddressOrPhoneNumber: The email address or phone number to send the reset code to.
         * 
         * Throws:
         * - ApplicationException if the email address or phone number is not found.
         * 
         *** */
        public async Task RequestResetPasswordAsync(string emailAddressOrPhoneNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddressOrPhoneNumber || u.PhoneNumber == emailAddressOrPhoneNumber);

            if (user == null)
            {
                throw new ApplicationException("Email address or phone number not found");
            }

            var authCode = await GenerateResetPasswordTokenAsync(user);

            var subject = "Password Reset Code";
            var msg_code = $"<h2>Your reset code is <br/><b>{authCode}</b></h2>";
            var expired = "The code will expire after <b>5 minutes</b>.";
            var noted = "Please pay attention to the usage time to avoid code expiration.";
            var system_msg = "Thank you for using our service.";

            var sms_msg_code = $"Your reset code is {authCode}.";
            var sms_expired = "The code will expire after 5 minutes.";

            if (emailAddressOrPhoneNumber.Contains("@"))
            {
                await _emailSender.SendEmailAsync(user.EmailAddress, subject, $"<div>{msg_code}<br/><hr/><h3>{expired}<br/>{noted}<br/>{system_msg}</h3></div>");
            }
            else
            {
                await _smsSender.SendSmsAsync(user.PhoneNumber, $"{sms_msg_code}\n{sms_expired}\n{noted}\n{system_msg}");
            }
        }
        /* ***
         * Resets the user's password using a valid reset token.
         * 
         * Parameters:
         * - resetPasswordDto: Contains the reset token and new password.
         * 
         * Throws:
         * - ApplicationException if the token is invalid or expired, or the user is not found.
         * 
         *** */
        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var token = await _context.ResetPasswordTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.PasswordResetCode == resetPasswordDto.PasswordToken && t.ExpirationTime > DateTime.UtcNow);

            if (token == null)
            {
                throw new ApplicationException("Invalid or expired reset code");
            }

            var user = token.User;

            if (user == null)
            {
                throw new ApplicationException("User not found for the provided token");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);

            
            var expiredTokens = await _context.ResetPasswordTokens
                .Where(t => t.UserId == user.UserId && t.ExpirationTime <= DateTime.UtcNow)
                .ToListAsync();

            // Remove all expired reset codes of the user
            _context.ResetPasswordTokens.RemoveRange(expiredTokens);
            //remove the current reser code
            _context.ResetPasswordTokens.Remove(token);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
