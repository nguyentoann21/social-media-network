using network_server.DTOs;
using network_server.Models;

namespace network_server.Services.s_user
{
    public interface IUserService
    {
        /* ***
        * Registers a new user with a specific role.
        * 
        * Parameters:
        * - registerDto: Contains the user's registration details.
        * - roleName: The role to assign to the user.
        * 
        * Returns:
        * - A UserDto representing the registered user.
        * 
        *** */
        Task<UserDto> RegisterAsync(RegisterDto registerDto, string roleName);
        /* ***
         * Registers a new user with default role.
         * 
         * Parameters:
         * - registerDto: Contains the user's registration details.
         * 
         * Returns:
         * - A UserDto representing the registered user.
         * 
         *** */
        Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
        /* ***
         * Authenticates a user and returns a JWT token.
         * 
         * Parameters:
         * - loginDto: Contains the user's login credentials (username/email and password).
         * 
         * Returns:
         * - A JWT token as a string if authentication is successful.
         * 
         *** */
        Task<string> LoginAsync(LoginDto loginDto);
        /* ***
         * Retrieves the role of a specific user.
         * 
         * Parameters:
         * - userId: The unique identifier of the user.
         * 
         * Returns:
         * - A string representing the role of the user.
         * 
         *** */
        Task<string> GetUserRoleAsync(Guid userId);
        /* ***
         * Assigns a role to a user.
         * 
         * Parameters:
         * - user: The user to whom the role is being assigned.
         * - roleName: The name of the role to assign.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task AssignRole(User user, string roleName);
        /* ***
         * Retrieves the profile details of a specific user.
         * 
         * Parameters:
         * - userId: The unique identifier of the user.
         * 
         * Returns:
         * - A UserProfileDto containing the user's profile information.
         * 
         *** */
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        /* ***
         * Updates the profile information of a specific user.
         * 
         * Parameters:
         * - userId: The unique identifier of the user.
         * - updateProfile: The details to update in the user's profile.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto updateProfile);
        /* ***
         * Updates the password of a specific user.
         * 
         * Parameters:
         * - userId: The unique identifier of the user.
         * - updatePassword: Contains the current and new password.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePassword);
        /* ***
         * Requests a password reset for a user based on email or phone number.
         * 
         * Parameters:
         * - emailAddressOrPhoneNumber: The email address or phone number of the user requesting a password reset.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task RequestResetPasswordAsync(string emailAddressOrPhoneNumber);
        /* ***
         * Resets the password for a user based on a reset token and new password.
         * 
         * Parameters:
         * - resetPasswordDto: Contains the email/phone number, reset token, and new password.
         * 
         * Returns:
         * - A Task representing the asynchronous operation.
         * 
         *** */
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
