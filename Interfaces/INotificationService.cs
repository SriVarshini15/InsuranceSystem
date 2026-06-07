using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.Models.Enums;

namespace VehicleInsuranceSystem.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, NotificationType type);

    Task NotifyWelcomeRegistrationAsync(int userId);

    Task NotifyEmailVerificationAsync(string email, string verificationLink);

    Task NotifyQuoteSavedAsync(int proposalId, string? renewalLink = null);

    Task NotifyPolicyIssuanceAsync(int proposalId);

    Task NotifyPolicyEndorsementAsync(int userId, string changedItem);

    Task NotifyPaymentSuccessAsync(int proposalId, decimal amount);

    Task NotifyPaymentFailureAsync(int proposalId, decimal amount);

    Task NotifyRenewalRemindersAsync(DateTime currentDate);

    Task NotifyPolicyLapsesAsync(DateTime currentDate);

    Task NotifyClaimIntakeAsync(int claimId);

    Task NotifyAdjusterAssignedAsync(int claimId, string adjusterName, string contactInfo);

    Task NotifyAdditionalDocumentsRequiredAsync(int claimId, string requiredDocuments);

    Task NotifyClaimDecisionAsync(int claimId, bool approved);

    Task NotifyClaimDisbursementAsync(int claimId, decimal amount, string destination);

    Task NotifyPasswordResetAsync(string email);

    Task NotifyTwoFactorAuthenticationAsync(string email);

    Task NotifySecurityAlertAsync(string email, string? deviceOrLocation);

    Task<PagedResultDto<NotificationDto>> GetUserNotificationsAsync(int userId, PaginationParamsDto paginationParams);

    Task<PagedResultDto<NotificationDto>> GetUserNotificationsByTypeAsync(int userId, string type, PaginationParamsDto paginationParams);

    Task<int> GetUnreadCountAsync(int userId);

    Task<bool> MarkAsReadAsync(int notificationId);

    Task MarkAllAsReadAsync(int userId);
}
