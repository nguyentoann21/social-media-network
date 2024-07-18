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

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmailAddress || u.EmailAddress == loginDto.UsernameOrEmailAddress);

            if (user == null) 
            {
                throw new ApplicationException("Invalid credentials");
            }

            var passwordHash = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!passwordHash)
            {
                throw new ApplicationException("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("EmailAddress", user.EmailAddress),
                new Claim("PhoneNumber", user.PhoneNumber ?? ""), // Đảm bảo PhoneNumber không null
                new Claim("Address", user.Address ?? ""), // Đảm bảo Address không null
                new Claim("Gender", user.Gender ?? ""), // Đảm bảo Gender không null
                new Claim("AvatarUrl", user.AvatarUrl ?? "") // Đảm bảo AvatarUrl không null
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

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.EmailAddress == registerDto.EmailAddress);
            
            if(user)
            {
                throw new ApplicationException("Username/EmailAddress already exist in another account");
            }

            //save file upload
            string avatarFile = null;
            if(registerDto.Avatar != null)
            {
                var storageFolder = Path.Combine("wwwroot", "avt");
                Directory.CreateDirectory(storageFolder);
                avatarFile = Path.Combine(storageFolder, registerDto.Avatar.FileName);
                
                using(var fileStream = new FileStream(avatarFile, FileMode.Create))
                {
                    await registerDto.Avatar.CopyToAsync(fileStream);
                }
            }

            var registerUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = registerDto.Username,
                AvatarUrl = avatarFile,
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
            return new UserDto
            {
                UserId = registerUser.UserId,
                Username = registerUser.Username,
                AvatarUrl = avatarFile,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                EmailAddress = registerUser.EmailAddress,
                PhoneNumber = registerUser.PhoneNumber,
                Address = registerUser.Address,
                Gender = registerUser.Gender
            };
        }
    }
}
