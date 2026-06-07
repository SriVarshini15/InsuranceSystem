using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PaymentDTOs;
using VehicleInsuranceSystem.Extensions;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new payment for policy quote (User action)
    /// </summary>
    /// <param name="createPaymentDto">Payment details</param>
    /// <returns>Payment confirmation</returns>
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid payment creation request");
            return BadRequest(new { error = "Invalid payment data", details = ModelState });
        }

        _logger.LogInformation($"Creating payment for proposal: {createPaymentDto.ProposalId}");
        var result = await _paymentService.CreatePaymentAsync(createPaymentDto);
        return Ok(new { message = "Payment created successfully", data = result });
    }

    /// <summary>
    /// Get payment details for a specific proposal
    /// </summary>
    /// <param name="proposalId">Proposal ID</param>
    /// <returns>Payment information</returns>
    [HttpGet("proposal/{proposalId}")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetPaymentByProposal(int proposalId)
    {
        try
        {
            _logger.LogInformation($"Fetching payment for proposal: {proposalId}");
            var result = await _paymentService.GetPaymentByProposalIdAsync(proposalId);
            
            if (result == null)
            {
                _logger.LogWarning($"Payment not found for proposal: {proposalId}");
                return NotFound(new { error = "Payment not found for this proposal" });
            }

            return Ok(new { message = "Payment fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching payment for proposal: {proposalId}");
            return StatusCode(500, new { error = "Unable to fetch payment" });
        }
    }

    /// <summary>
    /// Get all payments made by a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's payments</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetUserPayments(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching payments for user: {userId}");
            var result = await _paymentService.GetPaymentsByUserIdAsync(userId, paginationParams);
            return Ok(new { message = "Payments fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching payments for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch payments" });
        }
    }

    /// <summary>
    /// Confirm payment and update policy status (Officer action)
    /// </summary>
    /// <param name="paymentId">Payment ID</param>
    /// <returns>Confirmation result</returns>
    [HttpPut("{paymentId}/confirm")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ConfirmPayment(int paymentId)
    {
        try
        {
            _logger.LogInformation($"Officer confirming payment: {paymentId}");
            var result = await _paymentService.ConfirmPaymentAsync(paymentId);
            return Ok(new { message = "Payment confirmed successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error confirming payment: {paymentId}");
            return StatusCode(500, new { error = "Unable to confirm payment" });
        }
    }

    [HttpPut("{paymentId}/fail")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> MarkPaymentFailed(int paymentId)
    {
        try
        {
            _logger.LogInformation($"Officer marking payment failed: {paymentId}");
            var result = await _paymentService.MarkPaymentFailedAsync(paymentId);
            return Ok(new { message = "Payment failure notification sent", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking payment failed: {paymentId}");
            return StatusCode(500, new { error = "Unable to mark payment failed" });
        }
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    /// <param name="paymentId">Payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("{paymentId}")]
    [Authorize(Roles = "User,Officer")]
    public async Task<IActionResult> GetPayment(int paymentId)
    {
        try
        {
            _logger.LogInformation($"Fetching payment: {paymentId}");
            var result = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (result == null)
            {
                return NotFound(new { error = "Payment not found" });
            }

            return Ok(new { message = "Payment fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching payment: {paymentId}");
            return StatusCode(500, new { error = "Unable to fetch payment" });
        }
    }
}
