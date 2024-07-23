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
         * Constructor to initialize IUserService
         * 
         *** */
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /* ***
         * 
         * Registration endpoint allowing users to register with the role "User"
         * The data is submitted using [FromForm] to handle file uploads.
         * On success, returns the user data. On failure, returns a BadRequest with an error message.
         * 
         *** */
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
         * Registration endpoint for admins to create users with specified roles
         * Only accessible by users with the "Manager" role.
         * The data is submitted using [FromForm] for handling file uploads.
         * On success, returns the user data. On failure, returns a BadRequest with an error message.
         * 
         *** */
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
         * Login endpoint that returns a JWT token on success
         * On failure, returns an Unauthorized result with an error message.
         * 
         *** */
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
         * Endpoint to get the profile of the currently logged-in user
         * Requires authentication. Returns user data if successful.
         * On failure, returns an Unauthorized result with an error message.
         * 
         *** */
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
         * Endpoint to edit the profile of the currently logged-in user
         * Requires authentication. On success, returns a success message.
         * On failure, returns an Unauthorized result with an error message.
         * 
         *** */
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
         * Endpoint to change the password of the currently logged-in user
         * Requires authentication. On success, returns a success message.
         * On failure, returns an Unauthorized result with an error message.
         * 
         *** */
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
        /* ***
         * 
         * Endpoint to request a password reset code
         * Takes an email address or phone number to send the reset code.
         * On success, returns a message indicating the reset code has been sent.
         * On failure, returns a BadRequest with an error message.
         * 
         *** */
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string emailAddressOrPhoneNumber)
        {
            try
            {
                await _userService.RequestResetPasswordAsync(emailAddressOrPhoneNumber);
                return Ok("A reset code has been sent to your email address or phone number");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /* ***
         * 
         * Endpoint to reset the password using a reset code
         * Takes a ResetPasswordDto to perform the reset.
         * On success, returns a success message.
         * On failure, returns a BadRequest with an error message.
         * 
         *** */
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _userService.ResetPasswordAsync(resetPasswordDto);
                return Ok("Your password has been reset");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
