using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;
    private readonly ILogger<PoliciesController> _logger;

    public PoliciesController(IPolicyService policyService, ILogger<PoliciesController> logger)
    {
        _policyService = policyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available insurance policies
    /// </summary>
    /// <returns>List of all policies</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPolicies([FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation("Fetching all available policies");
            var policies = await _policyService.GetAllPoliciesAsync(paginationParams);
            return Ok(new { message = "Policies fetched successfully", data = policies });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all policies");
            return StatusCode(500, new { error = "Unable to fetch policies" });
        }
    }

    /// <summary>
    /// Get all active policies
    /// </summary>
    /// <returns>List of active policies</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePolicies([FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation("Fetching active policies");
            var policies = await _policyService.GetActivePoliciesAsync(paginationParams);
            return Ok(new { message = "Active policies fetched successfully", data = policies });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active policies");
            return StatusCode(500, new { error = "Unable to fetch active policies" });
        }
    }

    /// <summary>
    /// Get a specific policy by ID
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>Policy details</returns>
    [HttpGet("{policyId}")]
    public async Task<IActionResult> GetPolicy(int policyId)
    {
        try
        {
            _logger.LogInformation($"Fetching policy: {policyId}");
            var policy = await _policyService.GetPolicyByIdAsync(policyId);
            
            if (policy == null)
            {
                _logger.LogWarning($"Policy not found: {policyId}");
                return NotFound(new { error = "Policy not found" });
            }

            return Ok(new { message = "Policy fetched successfully", data = policy });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching policy: {policyId}");
            return StatusCode(500, new { error = "Unable to fetch policy" });
        }
    }

    /// <summary>
    /// Create a new insurance policy (Officer action)
    /// </summary>
    /// <param name="createPolicyDto">Policy details</param>
    /// <returns>Created policy information</returns>
    [HttpPost]
    [Authorize(Roles = "Officer")]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyDto createPolicyDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid policy creation request");
            return BadRequest(new { error = "Invalid policy data", details = ModelState });
        }

        _logger.LogInformation("Officer creating new policy");
        var result = await _policyService.CreatePolicyAsync(createPolicyDto);
        return Ok(new { message = "Policy created successfully", data = result });
    }

    /// <summary>
    /// Update an existing policy (Officer action)
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <param name="updatePolicyDto">Updated policy details</param>
    /// <returns>Update result</returns>
    [HttpPut("{policyId}")]
    [Authorize(Roles = "Officer")]
    public async Task<IActionResult> UpdatePolicy(int policyId, [FromBody] UpdatePolicyDto updatePolicyDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid policy update request");
            return BadRequest(new { error = "Invalid policy data", details = ModelState });
        }

        _logger.LogInformation($"Officer updating policy: {policyId}");
        var result = await _policyService.UpdatePolicyAsync(policyId, updatePolicyDto);
        return Ok(new { message = "Policy updated successfully", data = result });
    }

    /// <summary>
    /// Get all policies assigned to a user (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's policies with status</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserPolicies(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation($"Fetching policies for user: {userId}");
            var policies = await _policyService.GetUserPoliciesAsync(userId, paginationParams);
            return Ok(new { message = "User policies fetched successfully", data = policies });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user policies for: {userId}");
            return StatusCode(500, new { error = "Unable to fetch policies" });
        }
    }

    /// <summary>
    /// Download policy document (User action)
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>Policy document as PDF</returns>
    [HttpGet("assigned/{userPolicyId}/download")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> DownloadPolicyDocument(int userPolicyId)
    {
        try
        {
            _logger.LogInformation($"Downloading policy document for assigned policy: {userPolicyId}");
            var document = await _policyService.GeneratePolicyDocumentAsync(userPolicyId);
            if (document == null)
            {
                return NotFound(new { error = "Assigned policy not found" });
            }

            return File(document, "text/plain", $"policy-{userPolicyId}.txt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading policy document: {userPolicyId}");
            return StatusCode(500, new { error = "Unable to download policy document" });
        }
    }

    /// <summary>
    /// Activate a policy after payment confirmation (Officer action)
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>Activation result</returns>
    [HttpPut("{policyId}/activate")]
    [Authorize(Roles = "Officer")]
    public async Task<IActionResult> ActivatePolicy(int policyId)
    {
        try
        {
            _logger.LogInformation($"Officer activating policy: {policyId}");
            var result = await _policyService.SetPolicyActiveStatusAsync(policyId, true);
            return Ok(new { message = "Policy activated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating policy: {policyId}");
            return StatusCode(500, new { error = "Unable to activate policy" });
        }
    }

    /// <summary>
    /// Deactivate/Expire a policy (Officer action)
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>Deactivation result</returns>
    [HttpPut("{policyId}/deactivate")]
    [Authorize(Roles = "Officer")]
    public async Task<IActionResult> DeactivatePolicy(int policyId)
    {
        try
        {
            _logger.LogInformation($"Officer deactivating policy: {policyId}");
            var result = await _policyService.SetPolicyActiveStatusAsync(policyId, false);
            return Ok(new { message = "Policy deactivated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating policy: {policyId}");
            return StatusCode(500, new { error = "Unable to deactivate policy" });
        }
    }
}
