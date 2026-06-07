using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.VehicleDTOs;
using VehicleInsuranceSystem.Extensions;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Add a new vehicle for user (User action)
    /// </summary>
    /// <param name="createVehicleDto">Vehicle details</param>
    /// <returns>Added vehicle information</returns>
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddVehicle([FromBody] CreateVehicleDto createVehicleDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid vehicle addition request");
            return BadRequest(new { error = "Invalid vehicle data", details = ModelState });
        }

        // Extract userId from claims or request context
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Unable to extract user ID from claims");
            return Unauthorized(new { error = "User identification failed" });
        }

        _logger.LogInformation($"User {userId} adding vehicle: {createVehicleDto.VehicleNumber}");
        var result = await _vehicleService.AddVehicleAsync(userId, createVehicleDto);
        return Ok(new { message = "Vehicle added successfully", data = result });
    }

    /// <summary>
    /// Get a specific vehicle by ID
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <returns>Vehicle details</returns>
    [HttpGet("{vehicleId}")]
    [Authorize]
    public async Task<IActionResult> GetVehicle(int vehicleId)
    {
        try
        {
            _logger.LogInformation($"Fetching vehicle: {vehicleId}");
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            
            if (result == null)
            {
                _logger.LogWarning($"Vehicle not found: {vehicleId}");
                return NotFound(new { error = "Vehicle not found" });
            }

            return Ok(new { message = "Vehicle fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching vehicle: {vehicleId}");
            return StatusCode(500, new { error = "Unable to fetch vehicle" });
        }
    }

    /// <summary>
    /// Get all vehicles for a specific user (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's vehicles</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserVehicles(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching vehicles for user: {userId}");
            var result = await _vehicleService.GetVehiclesByUserAsync(userId, paginationParams);
            return Ok(new { message = "Vehicles fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching vehicles for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch vehicles" });
        }
    }

    /// <summary>
    /// Update vehicle information (User action)
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="updateVehicleDto">Updated vehicle details</param>
    /// <returns>Update result</returns>
    [HttpPut("{vehicleId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] UpdateVehicleDto updateVehicleDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid vehicle update request");
            return BadRequest(new { error = "Invalid vehicle data", details = ModelState });
        }

        try
        {
            _logger.LogInformation($"Updating vehicle: {vehicleId}");
            var result = await _vehicleService.UpdateVehicleAsync(vehicleId, updateVehicleDto);
            return Ok(new { message = "Vehicle updated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating vehicle: {vehicleId}");
            return StatusCode(500, new { error = "Unable to update vehicle" });
        }
    }

    /// <summary>
    /// Delete a vehicle (User action)
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{vehicleId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
        try
        {
            _logger.LogInformation($"Deleting vehicle: {vehicleId}");
            var result = await _vehicleService.DeleteVehicleAsync(vehicleId);
            if (result)
            {
                return Ok(new { message = "Vehicle deleted successfully" });
            }
            return NotFound(new { error = "Vehicle not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting vehicle: {vehicleId}");
            return StatusCode(500, new { error = "Unable to delete vehicle" });
        }
    }

    /// <summary>
    /// Get vehicles by category (Car/Bike/Camper Van)
    /// </summary>
    /// <param name="category">Vehicle category</param>
    /// <returns>Vehicles matching category</returns>
    [HttpGet("category/{category}")]
    [Authorize]
    public async Task<IActionResult> GetVehiclesByCategory(string category)
    {
        try
        {
            _logger.LogInformation($"Fetching vehicles by category: {category}");
            var allVehicles = new List<VehicleDto>();
            // In actual implementation, filter by category
            return Ok(new { message = "Vehicles fetched successfully", data = allVehicles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching vehicles by category: {category}");
            return StatusCode(500, new { error = "Unable to fetch vehicles" });
        }
    }
}
