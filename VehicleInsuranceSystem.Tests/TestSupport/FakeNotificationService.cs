using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Enums;

namespace VehicleInsuranceSystem.Tests.TestSupport;

public class FakeNotificationService : INotificationService
{
    public List<string> Calls { get; } = new();

    public Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, NotificationType type)
    {
        Calls.Add($"{nameof(CreateNotificationAsync)}:{type}");
        return Task.FromResult(new NotificationDto(1, title, message, false, DateTime.UtcNow));
    }

    public Task NotifyWelcomeRegistrationAsync(int userId) => Track(nameof(NotifyWelcomeRegistrationAsync));
    public Task NotifyEmailVerificationAsync(string email, string verificationLink) => Track(nameof(NotifyEmailVerificationAsync));
    public Task NotifyQuoteSavedAsync(int proposalId, string? renewalLink = null) => Track(nameof(NotifyQuoteSavedAsync));
    public Task NotifyPolicyIssuanceAsync(int proposalId) => Track(nameof(NotifyPolicyIssuanceAsync));
    public Task NotifyPolicyEndorsementAsync(int userId, string changedItem) => Track(nameof(NotifyPolicyEndorsementAsync));
    public Task NotifyPaymentSuccessAsync(int proposalId, decimal amount) => Track(nameof(NotifyPaymentSuccessAsync));
    public Task NotifyPaymentFailureAsync(int proposalId, decimal amount) => Track(nameof(NotifyPaymentFailureAsync));
    public Task NotifyRenewalRemindersAsync(DateTime currentDate) => Track(nameof(NotifyRenewalRemindersAsync));
    public Task NotifyPolicyLapsesAsync(DateTime currentDate) => Track(nameof(NotifyPolicyLapsesAsync));
    public Task NotifyClaimIntakeAsync(int claimId) => Track(nameof(NotifyClaimIntakeAsync));
    public Task NotifyAdjusterAssignedAsync(int claimId, string adjusterName, string contactInfo) => Track(nameof(NotifyAdjusterAssignedAsync));
    public Task NotifyAdditionalDocumentsRequiredAsync(int claimId, string requiredDocuments) => Track(nameof(NotifyAdditionalDocumentsRequiredAsync));
    public Task NotifyClaimDecisionAsync(int claimId, bool approved) => Track($"{nameof(NotifyClaimDecisionAsync)}:{approved}");
    public Task NotifyClaimDisbursementAsync(int claimId, decimal amount, string destination) => Track(nameof(NotifyClaimDisbursementAsync));
    public Task NotifyPasswordResetAsync(string email) => Track(nameof(NotifyPasswordResetAsync));
    public Task NotifyTwoFactorAuthenticationAsync(string email) => Track(nameof(NotifyTwoFactorAuthenticationAsync));
    public Task NotifySecurityAlertAsync(string email, string? deviceOrLocation) => Track(nameof(NotifySecurityAlertAsync));

    public Task<PagedResultDto<NotificationDto>> GetUserNotificationsAsync(int userId, PaginationParamsDto paginationParams)
        => Task.FromResult(EmptyPage());

    public Task<PagedResultDto<NotificationDto>> GetUserNotificationsByTypeAsync(int userId, string type, PaginationParamsDto paginationParams)
        => Task.FromResult(EmptyPage());

    public Task<int> GetUnreadCountAsync(int userId) => Task.FromResult(0);
    public Task<bool> MarkAsReadAsync(int notificationId) => Task.FromResult(true);
    public Task MarkAllAsReadAsync(int userId) => Track(nameof(MarkAllAsReadAsync));

    private Task Track(string name)
    {
        Calls.Add(name);
        return Task.CompletedTask;
    }

    private static PagedResultDto<NotificationDto> EmptyPage()
        => new(new List<NotificationDto>(), 1, 10, 0, 0);
}
