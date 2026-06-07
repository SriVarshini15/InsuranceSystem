using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.ClaimDTOs;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.Extensions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Enums;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;
    private readonly ILogger<ClaimsController> _logger;

    public ClaimsController(IClaimService claimService, ILogger<ClaimsController> logger)
    {
        _claimService = claimService;
        _logger = logger;
    }

    /// <summary>
    /// File a new claim for active policy (User action)
    /// </summary>
    /// <param name="createClaimDto">Claim details</param>
    /// <returns>Filed claim confirmation</returns>
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateClaim([FromBody] CreateClaimDto createClaimDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid claim creation request");
            return BadRequest(new { error = "Invalid claim data", details = ModelState });
        }

        _logger.LogInformation($"User filing claim for policy: {createClaimDto.UserPolicyId}");
        var result = await _claimService.CreateClaimAsync(createClaimDto);
        return Ok(new { message = "Claim filed successfully", data = result });
    }

    /// <summary>
    /// Get all claims for a specific policy (Officer action)
    /// </summary>
    /// <param name="userPolicyId">User Policy ID</param>
    /// <returns>List of claims for policy</returns>
    [HttpGet("policy/{userPolicyId}")]
    [Authorize(Roles = "Officer,User")]
    public async Task<IActionResult> GetClaimsByPolicy(int userPolicyId, [FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation($"Fetching claims for policy: {userPolicyId}");
            var result = await _claimService.GetClaimsByPolicyIdAsync(userPolicyId, paginationParams);
            return Ok(new { message = "Claims fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching claims for policy: {userPolicyId}");
            return StatusCode(500, new { error = "Unable to fetch claims" });
        }
    }

    /// <summary>
    /// Get all claims filed by a user (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's claims</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetClaimsByUser(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching claims for user: {userId}");
            var result = await _claimService.GetClaimsByUserIdAsync(userId, paginationParams);
            return Ok(new { message = "User claims fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching claims for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch claims" });
        }
    }

    /// <summary>
    /// Get a specific claim by ID
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <returns>Claim details</returns>
    [HttpGet("{claimId}")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetClaim(int claimId)
    {
        try
        {
            _logger.LogInformation($"Fetching claim: {claimId}");
            var result = await _claimService.GetClaimByIdAsync(claimId);
            if (result == null)
            {
                return NotFound(new { error = "Claim not found" });
            }

            return Ok(new { message = "Claim fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching claim: {claimId}");
            return StatusCode(500, new { error = "Unable to fetch claim" });
        }
    }

    /// <summary>
    /// Update claim status (Officer action)
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="newStatus">New claim status (Approved/Rejected/Pending)</param>
    /// <returns>Status update result</returns>
    [HttpPut("{claimId}/status")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> UpdateClaimStatus(int claimId, [FromBody] UpdateClaimStatusDto updateStatusDto)
    {
        if (updateStatusDto == null || string.IsNullOrWhiteSpace(updateStatusDto.Status))
        {
            _logger.LogWarning("Invalid claim status update request");
            return BadRequest(new { error = "New status is required" });
        }

        try
        {
            if (!Enum.TryParse<ClaimStatus>(updateStatusDto.Status, true, out var status))
            {
                return BadRequest(new { error = "Invalid claim status" });
            }

            _logger.LogInformation($"Officer updating claim status for: {claimId} to {updateStatusDto.Status}");
            var result = await _claimService.UpdateClaimStatusAsync(claimId, status);
            return Ok(new { message = "Claim status updated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating claim status: {claimId}");
            return StatusCode(500, new { error = "Unable to update claim status" });
        }
    }

    /// <summary>
    /// Accept/Approve a claim (Officer action)
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <returns>Approval result</returns>
    [HttpPut("{claimId}/approve")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ApproveClaim(int claimId)
    {
        try
        {
            _logger.LogInformation($"Officer approving claim: {claimId}");
            var result = await _claimService.UpdateClaimStatusAsync(claimId, ClaimStatus.Approved);
            return Ok(new { message = "Claim approved successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving claim: {claimId}");
            return StatusCode(500, new { error = "Unable to approve claim" });
        }
    }

    /// <summary>
    /// Reject a claim (Officer action)
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <returns>Rejection result</returns>
    [HttpPut("{claimId}/reject")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> RejectClaim(int claimId)
    {
        try
        {
            _logger.LogInformation($"Officer rejecting claim: {claimId}");
            var result = await _claimService.UpdateClaimStatusAsync(claimId, ClaimStatus.Rejected);
            return Ok(new { message = "Claim rejected successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting claim: {claimId}");
            return StatusCode(500, new { error = "Unable to reject claim" });
        }
    }

    /// <summary>
    /// Process claim payment (Officer action)
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <returns>Payment processing result</returns>
    [HttpPut("{claimId}/process-payment")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ProcessClaimPayment(int claimId)
    {
        try
        {
            _logger.LogInformation($"Officer processing payment for claim: {claimId}");
            var result = await _claimService.UpdateClaimStatusAsync(claimId, ClaimStatus.Paid);
            return Ok(new { message = "Claim payment processed successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing claim payment: {claimId}");
            return StatusCode(500, new { error = "Unable to process claim payment" });
        }
    }
}
