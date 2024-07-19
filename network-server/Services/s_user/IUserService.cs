using network_server.DTOs;
using network_server.Models;

namespace network_server.Services.s_user
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterDto registerDto, string roleName);
        Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<string> GetUserRoleAsync(Guid userId);
        Task AssignRole(User user, string roleName);
    }
}
