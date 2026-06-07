using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.Extensions;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get user profile details by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile information</returns>
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUser(int userId)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching user profile: {userId}");
            var result = await _userService.GetUserByIdAsync(userId);
            
            if (result == null)
            {
                _logger.LogWarning($"User not found: {userId}");
                return NotFound(new { error = "User not found" });
            }

            return Ok(new { message = "User fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch user profile" });
        }
    }

    /// <summary>
    /// Get all users (Officer/Admin action)
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation("Fetching all users");
            var result = await _userService.GetAllUsersAsync(paginationParams);
            return Ok(new { message = "Users fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return StatusCode(500, new { error = "Unable to fetch users" });
        }
    }

    /// <summary>
    /// Update user profile information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="updateUserDto">Updated user details</param>
    /// <returns>Update result</returns>
    [HttpPut("{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid user update request");
            return BadRequest(new { error = "Invalid user data", details = ModelState });
        }

        try
        {
            _logger.LogInformation($"Updating user profile: {userId}");
            var result = await _userService.UpdateUserAsync(userId, updateUserDto);
            return Ok(new { message = "User updated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user: {userId}");
            return StatusCode(500, new { error = "Unable to update user profile" });
        }
    }

    /// <summary>
    /// Delete user account (Officer/Admin action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{userId}")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            _logger.LogInformation($"Deleting user account: {userId}");
            var result = await _userService.DeleteUserAsync(userId);
            if (result)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            return NotFound(new { error = "User not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user: {userId}");
            return StatusCode(500, new { error = "Unable to delete user account" });
        }
    }

    /// <summary>
    /// Get complete user profile (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Complete user profile with policies and claims</returns>
    [HttpGet("{userId}/profile")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching complete profile for user: {userId}");
            var result = await _userService.GetUserByIdAsync(userId);
            
            if (result == null)
            {
                _logger.LogWarning($"User profile not found: {userId}");
                return NotFound(new { error = "User profile not found" });
            }

            return Ok(new { message = "User profile fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user profile: {userId}");
            return StatusCode(500, new { error = "Unable to fetch user profile" });
        }
    }

    /// <summary>
    /// Deactivate user account (Officer/Admin action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Deactivation result</returns>
    [HttpPut("{userId}/deactivate")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        try
        {
            _logger.LogInformation($"Deactivating user account: {userId}");
            // Since UpdateUserDto doesn't have IsActive, fetch current user first
            var currentUser = await _userService.GetUserByIdAsync(userId);
            if (currentUser == null)
            {
                _logger.LogWarning($"User not found: {userId}");
                return NotFound(new { error = "User not found" });
            }
            var deactivateDto = new UpdateUserDto(currentUser.FirstName, currentUser.LastName, currentUser.PhoneNumber, currentUser.Address);
            var result = await _userService.UpdateUserAsync(userId, deactivateDto);
            return Ok(new { message = "User deactivated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating user: {userId}");
            return StatusCode(500, new { error = "Unable to deactivate user account" });
        }
    }

    /// <summary>
    /// Activate user account (Officer/Admin action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Activation result</returns>
    [HttpPut("{userId}/activate")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        try
        {
            _logger.LogInformation($"Activating user account: {userId}");
            // Fetch current user to get existing values
            var currentUser = await _userService.GetUserByIdAsync(userId);
            if (currentUser == null)
            {
                _logger.LogWarning($"User not found: {userId}");
                return NotFound(new { error = "User not found" });
            }
            var updateDto = new UpdateUserDto(currentUser.FirstName, currentUser.LastName, currentUser.PhoneNumber, currentUser.Address);
            var result = await _userService.UpdateUserAsync(userId, updateDto);
            return Ok(new { message = "User activated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating user: {userId}");
            return StatusCode(500, new { error = "Unable to activate user account" });
        }
    }
}
