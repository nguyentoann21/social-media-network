using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using network_server.DTOs;
using network_server.Services.s_user;

namespace network_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                var user = await _userService.RegisterAsync(registerDto);
                return Ok(user);
            } 
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginAsync(loginDto);
                return Ok(new { token });
            }
            catch (ApplicationException ex) 
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
