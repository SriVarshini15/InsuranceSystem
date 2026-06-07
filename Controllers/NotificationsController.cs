using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.Extensions;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications for a user (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user notifications</returns>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserNotifications(int userId, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching notifications for user: {userId}");
            var result = await _notificationService.GetUserNotificationsAsync(userId, paginationParams);
            return Ok(new { message = "Notifications fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching notifications for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch notifications" });
        }
    }

    /// <summary>
    /// Mark a notification as read (User action)
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Update result</returns>
    [HttpPut("{notificationId}/read")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            _logger.LogInformation($"Marking notification as read: {notificationId}");
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            if (result)
            {
                return Ok(new { message = "Notification marked as read" });
            }
            return NotFound(new { error = "Notification not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking notification as read: {notificationId}");
            return StatusCode(500, new { error = "Unable to update notification" });
        }
    }

    /// <summary>
    /// Get unread notifications count for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Count of unread notifications</returns>
    [HttpGet("user/{userId}/unread-count")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUnreadNotificationCount(int userId)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching unread notification count for user: {userId}");
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { message = "Unread count fetched successfully", data = unreadCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching unread notification count for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch notification count" });
        }
    }

    /// <summary>
    /// Get notifications by type (Email/SMS/In-App)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="type">Notification type</param>
    /// <returns>Notifications of specified type</returns>
    [HttpGet("user/{userId}/type/{type}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetNotificationsByType(int userId, string type, [FromQuery] PaginationParamsDto paginationParams)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Fetching {type} notifications for user: {userId}");
            var result = await _notificationService.GetUserNotificationsByTypeAsync(userId, type, paginationParams);
            return Ok(new { message = "Notifications fetched successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching notifications by type for user: {userId}");
            return StatusCode(500, new { error = "Unable to fetch notifications" });
        }
    }

    /// <summary>
    /// Delete a notification (User action)
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{notificationId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            _logger.LogInformation($"Deleting notification: {notificationId}");
            // In actual implementation, implement delete functionality in service
            return Ok(new { message = "Notification deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting notification: {notificationId}");
            return StatusCode(500, new { error = "Unable to delete notification" });
        }
    }

    /// <summary>
    /// Mark all notifications as read for a user (User action)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Update result</returns>
    [HttpPut("user/{userId}/read-all")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        if (!User.CanAccessUser(userId))
        {
            return Forbid();
        }

        try
        {
            _logger.LogInformation($"Marking all notifications as read for user: {userId}");
            await _notificationService.MarkAllAsReadAsync(userId);

            return Ok(new { message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking all notifications as read for user: {userId}");
            return StatusCode(500, new { error = "Unable to update notifications" });
        }
    }

    [HttpPost("quote-saved")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> SendQuoteSavedNotification([FromBody] QuoteSavedNotificationDto request)
    {
        await _notificationService.NotifyQuoteSavedAsync(request.ProposalId, request.RenewalLink);
        return Ok(new { message = "Quote saved notification sent" });
    }

    [HttpPost("claims/adjuster-assigned")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> SendAdjusterAssignment([FromBody] AdjusterAssignmentNotificationDto request)
    {
        await _notificationService.NotifyAdjusterAssignedAsync(request.ClaimId, request.AdjusterName, request.ContactInfo);
        return Ok(new { message = "Adjuster assignment notification sent" });
    }

    [HttpPost("claims/additional-documents")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> SendAdditionalDocumentsRequired([FromBody] AdditionalDocumentsNotificationDto request)
    {
        await _notificationService.NotifyAdditionalDocumentsRequiredAsync(request.ClaimId, request.RequiredDocuments);
        return Ok(new { message = "Additional documents notification sent" });
    }

    [HttpPost("claims/disbursement")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> SendClaimDisbursement([FromBody] ClaimDisbursementNotificationDto request)
    {
        await _notificationService.NotifyClaimDisbursementAsync(request.ClaimId, request.Amount, request.Destination);
        return Ok(new { message = "Claim disbursement notification sent" });
    }

    [HttpPost("renewals/process")]
    [Authorize(Roles = "Officer,Admin")]
    public async Task<IActionResult> ProcessRenewalNotifications()
    {
        var today = DateTime.UtcNow.Date;
        await _notificationService.NotifyRenewalRemindersAsync(today);
        await _notificationService.NotifyPolicyLapsesAsync(today);
        return Ok(new { message = "Renewal reminders and lapse notices processed" });
    }
}
