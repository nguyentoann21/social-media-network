using Microsoft.AspNetCore.Authorization;
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

        /* ***
         * 
         * Constructor
         * 
         * ***/
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /* ***
         * 
         * Registration with role User only using [FromForm] to use the static file
         * Registration success, the system will return the user data
         * Register failed, the system that will display the error based on reasonable casese
         * 
         * ***/
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(registerDto);
                return Ok(user);
            } 
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* ***
         * 
         * Only Manager can take this action
         * Create a user with a role added by the admin, only using [FromForm] to use the static file
         * Create a user success, the system will return the user data
         * Create a user failed, the system that will display the error based on reasonable casese
         * 
         * ***/
        [Authorize(Policy = "Manager")]
        [HttpPost("create-user")]
        public async Task<IActionResult> RegisterWithRole([FromForm] RegisterDto registerDto, [FromQuery] string roleName)
        {
            try
            {
                var user = await _userService.RegisterAsync(registerDto, roleName);
                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* ***
         * 
         * Login success, the system will return a token
         * Login failed, the system that will display the error based on reasonable casese
         * 
         * ***/
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

        /* ***
         * 
         * Need to login first
         * That action will return user-authorized data
         * 
         * ***/
        [Authorize]
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value;

                if (userId == null)
                {
                    return Unauthorized("You need to log in first to take this action");
                }

                var profile = await _userService.GetUserProfileAsync(Guid.Parse(userId));

                return Ok(profile);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /* ***
         * 
         * Need to login first
         * That action allows users to edit their profiles.
         * Edit success, the system will return the text. It is "Your profile has been changed"
         * Edit failed, the system that will display the error based on reasonable casese
         * 
         * ***/
        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromForm] UpdateProfileDto updateProfile)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value;

                if (userId == null)
                {
                    return Unauthorized("You need to log in first to take this action");
                }

                await _userService.UpdateProfileAsync(Guid.Parse(userId), updateProfile);

                return Ok("Your profile has been changed");
            }
            catch(ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /* ***
         * 
         * Need to login first
         * That action allows users to edit their passwords.
         * Edit success, the system will return the text. It is "Your profile has been changed".
         * Edit failed, the system that will display the error based on reasonable casese.
         * 
         * ***/
        [Authorize]
        [HttpPut("edit-password")]
        public async Task<IActionResult> EditPassword([FromBody] UpdatePasswordDto updatePassword)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value;

                if (userId == null)
                {
                    return Unauthorized("You need to log in first to take this action");
                }

                await _userService.UpdatePasswordAsync(Guid.Parse(userId), updatePassword);

                return Ok("Your password has been changed");
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
