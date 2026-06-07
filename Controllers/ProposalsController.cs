using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.ProposalDTOs;
using VehicleInsuranceSystem.Extensions;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class ProposalsController : ControllerBase
{
    private readonly IProposalService _proposalService;
    private readonly ILogger<ProposalsController> _logger;

    public ProposalsController(IProposalService proposalService, ILogger<ProposalsController> logger)
    {
        _proposalService = proposalService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new insurance policy proposal (User action)
    /// </summary>
    /// <param name="createProposalDto">Proposal details</param>
    /// <returns>Created proposal information</returns>
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateProposal([FromBody] CreateProposalDto createProposalDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid proposal creation request");
            return BadRequest(new { error = "Invalid proposal data", details = ModelState });
        }

        // Extract userId from claims
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Unable to extract user ID from claims");
            return Unauthorized(new { error = "User identification failed" });
        }

        _logger.LogInformation($"Creating proposal for user: {userId}");
        var result = await _proposalService.CreateProposalAsync(userId, createProposalDto);
        return Ok(new { message = "Proposal created successfully", data = result });
    }

    /// <summary>
    /// Get all proposals submitted by a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's proposals</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserProposals(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching proposals for user: {userId}");
            var result = await _proposalService.GetUserProposalsAsync(userId, paginationParams);
            return Ok(new { message = "Proposals fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user proposals for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch proposals" });
        }
    }

    /// <summary>
    /// Get proposal details by ID (User or Officer)
    /// </summary>
    /// <param name="proposalId">Proposal ID</param>
    /// <returns>Proposal details</returns>
    [HttpGet("{proposalId}")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetProposal(int proposalId)
    {
        try
        {
            _logger.LogInformation($"Fetching proposal: {proposalId}");
            var result = await _proposalService.GetProposalStatusAsync(proposalId);
            if (result == null)
            {
                return NotFound(new { error = "Proposal not found" });
            }
            return Ok(new { message = "Proposal fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching proposal: {proposalId}");
            return StatusCode(500, new { error = "Unable to fetch proposal" });
        }
    }

    /// <summary>
    /// Get all proposals for officer review
    /// </summary>
    /// <returns>List of all proposals</returns>
    [HttpGet]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> GetAllProposals([FromQuery] PaginationParamsDto paginationParams)
    {
        try
        {
            _logger.LogInformation("Fetching all proposals for officer review");
            var result = await _proposalService.GetAllProposalsAsync(paginationParams);
            return Ok(new { message = "Proposals fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all proposals");
            return StatusCode(500, new { error = "Unable to fetch proposals" });
        }
    }

    /// <summary>
    /// Review proposal and request additional details (Officer action)
    /// </summary>
    /// <param name="proposalId">Proposal ID</param>
    /// <param name="reviewDto">Review details with comments/requests</param>
    /// <returns>Review result</returns>
    [HttpPut("{proposalId}/review")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ReviewProposal(int proposalId, [FromBody] ReviewProposalDto reviewDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid proposal review request");
            return BadRequest(new { error = "Invalid review data", details = ModelState });
        }

        _logger.LogInformation($"Officer reviewing proposal: {proposalId}");
        var result = await _proposalService.ReviewProposalAsync(proposalId, reviewDto);
        return Ok(new { message = "Proposal reviewed successfully", data = result });
    }

    /// <summary>
    /// Generate quote for approved proposal (Officer action)
    /// </summary>
    /// <param name="createQuoteDto">Quote calculation details</param>
    /// <returns>Generated quote</returns>
    [HttpPost("generate-quote")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> GenerateQuote([FromBody] CreateQuoteDto createQuoteDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid quote generation request");
            return BadRequest(new { error = "Invalid quote data", details = ModelState });
        }

        _logger.LogInformation("Officer generating quote");
        var result = await _proposalService.GenerateQuoteAsync(createQuoteDto);
        return Ok(new { message = "Quote generated successfully", data = result });
    }

    /// <summary>
    /// Get generated quote for a proposal
    /// </summary>
    /// <param name="proposalId">Proposal ID</param>
    /// <returns>Quote details</returns>
    [HttpGet("{proposalId}/quote")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetQuote(int proposalId)
    {
        try
        {
            _logger.LogInformation($"Fetching quote for proposal: {proposalId}");
            var result = await _proposalService.GetQuoteByProposalIdAsync(proposalId);
            return Ok(new { message = "Quote fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching quote for proposal: {proposalId}");
            return StatusCode(500, new { error = "Unable to fetch quote" });
        }
    }

    /// <summary>
    /// Get proposal status
    /// </summary>
    /// <param name="proposalId">Proposal ID</param>
    /// <returns>Current proposal status</returns>
    [HttpGet("{proposalId}/status")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetProposalStatus(int proposalId)
    {
        try
        {
            _logger.LogInformation($"Fetching status for proposal: {proposalId}");
            var result = await _proposalService.GetProposalStatusAsync(proposalId);
            return Ok(new { message = "Proposal status fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching proposal status: {proposalId}");
            return StatusCode(500, new { error = "Unable to fetch proposal status" });
        }
    }
}
