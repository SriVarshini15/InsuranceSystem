using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Notifications;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ApplicationDbContext context,
        IMapper mapper,
        IConfiguration config,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _config = config;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, NotificationType type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task NotifyWelcomeRegistrationAsync(int userId)
    {
        await CreateNotificationAsync(
            userId,
            "Welcome to Vehicle Insurance",
            "Your account has been created. Please verify your email address to complete registration.",
            NotificationType.WelcomeRegistration);
    }

    public async Task NotifyEmailVerificationAsync(string email, string verificationLink)
    {
        await SendEmailAsync(
            email,
            "Verify your Vehicle Insurance account",
            $"Please verify your email address by opening this link: {verificationLink}");
    }

    public async Task NotifyQuoteSavedAsync(int proposalId, string? renewalLink = null)
    {
        var proposal = await _context.Proposals
            .Include(x => x.Policy)
            .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        if (proposal == null)
        {
            return;
        }

        var actionText = string.IsNullOrWhiteSpace(renewalLink)
            ? "Sign in to complete your purchase."
            : $"Complete your purchase here: {renewalLink}";

        await CreateNotificationAsync(
            proposal.UserId,
            "Your quote is saved",
            $"Your quote for {proposal.Policy.PolicyName} is saved. {actionText}",
            NotificationType.QuoteSaved);
    }

    public async Task NotifyPolicyIssuanceAsync(int proposalId)
    {
        var proposal = await _context.Proposals
            .Include(x => x.Policy)
            .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        if (proposal == null)
        {
            return;
        }

        await CreateNotificationAsync(
            proposal.UserId,
            "Policy issued",
            $"Your {proposal.Policy.PolicyName} policy has been issued. Policy Schedule, Certificate of Insurance, and Terms & Conditions are available in your policy documents.",
            NotificationType.PolicyIssued);
    }

    public async Task NotifyPolicyEndorsementAsync(int userId, string changedItem)
    {
        await CreateNotificationAsync(
            userId,
            "Policy endorsement recorded",
            $"Your {changedItem} information was updated and the policy endorsement has been recorded.",
            NotificationType.PolicyEndorsement);
    }

    public async Task NotifyPaymentSuccessAsync(int proposalId, decimal amount)
    {
        var proposal = await _context.Proposals.FindAsync(proposalId);
        if (proposal == null)
        {
            return;
        }

        await CreateNotificationAsync(
            proposal.UserId,
            "Payment successful",
            $"We received your payment of {amount:C}. Your coverage status will be updated shortly.",
            NotificationType.PaymentReceived);
    }

    public async Task NotifyPaymentFailureAsync(int proposalId, decimal amount)
    {
        var proposal = await _context.Proposals.FindAsync(proposalId);
        if (proposal == null)
        {
            return;
        }

        await CreateNotificationAsync(
            proposal.UserId,
            "Payment failed",
            $"Your payment of {amount:C} could not be processed. Please retry to avoid a potential coverage lapse.",
            NotificationType.PaymentFailed);
    }

    public async Task NotifyRenewalRemindersAsync(DateTime currentDate)
    {
        var reminderDays = new[] { 30, 14, 1 };
        foreach (var days in reminderDays)
        {
            var expiryDate = currentDate.Date.AddDays(days);
            var userPolicies = await _context.UserPolicies
                .Include(x => x.Proposal)
                    .ThenInclude(x => x.Policy)
                .Where(x => x.EndDate.Date == expiryDate && x.Status != "Lapsed")
                .ToListAsync();

            foreach (var userPolicy in userPolicies)
            {
                var title = days == 1 ? "Coverage expires tomorrow" : $"{days}-day renewal reminder";
                var message = days == 1
                    ? $"Your {userPolicy.Proposal.Policy.PolicyName} coverage expires tomorrow. Renew now to keep your vehicle covered."
                    : $"Your {userPolicy.Proposal.Policy.PolicyName} policy expires on {userPolicy.EndDate:yyyy-MM-dd}. Review the new premium and renew before expiry.";

                await CreateNotificationAsync(
                    userPolicy.Proposal.UserId,
                    title,
                    message,
                    NotificationType.PolicyExpiryReminder);
            }
        }
    }

    public async Task NotifyPolicyLapsesAsync(DateTime currentDate)
    {
        var lapsedDate = currentDate.Date.AddDays(-1);
        var userPolicies = await _context.UserPolicies
            .Include(x => x.Proposal)
                .ThenInclude(x => x.Policy)
            .Where(x => x.EndDate.Date == lapsedDate && x.Status != "Lapsed")
            .ToListAsync();

        foreach (var userPolicy in userPolicies)
        {
            userPolicy.Status = "Lapsed";
            await CreateNotificationAsync(
                userPolicy.Proposal.UserId,
                "Policy lapsed",
                $"Your {userPolicy.Proposal.Policy.PolicyName} policy expired on {userPolicy.EndDate:yyyy-MM-dd}. You are no longer legally covered to drive under this policy.",
                NotificationType.PolicyLapsed);
        }

        await _context.SaveChangesAsync();
    }

    public async Task NotifyClaimIntakeAsync(int claimId)
    {
        var claim = await _context.Claims
            .Include(x => x.UserPolicy)
                .ThenInclude(x => x.Proposal)
            .FirstOrDefaultAsync(x => x.ClaimId == claimId);

        if (claim == null)
        {
            return;
        }

        await CreateNotificationAsync(
            claim.UserPolicy.Proposal.UserId,
            "Claim received",
            $"Your claim has been filed. Claim Reference Number: {claim.ClaimNumber}. Our team will review it and share next steps.",
            NotificationType.ClaimSubmitted);
    }

    public async Task NotifyAdjusterAssignedAsync(int claimId, string adjusterName, string contactInfo)
    {
        var claim = await GetClaimWithUserAsync(claimId);
        if (claim == null)
        {
            return;
        }

        await CreateNotificationAsync(
            claim.UserPolicy.Proposal.UserId,
            "Surveyor assigned",
            $"{adjusterName} has been assigned to inspect your vehicle damage. Contact: {contactInfo}.",
            NotificationType.AdjusterAssigned);
    }

    public async Task NotifyAdditionalDocumentsRequiredAsync(int claimId, string requiredDocuments)
    {
        var claim = await GetClaimWithUserAsync(claimId);
        if (claim == null)
        {
            return;
        }

        await CreateNotificationAsync(
            claim.UserPolicy.Proposal.UserId,
            "Additional documents required",
            $"We need additional documents for claim {claim.ClaimNumber}: {requiredDocuments}.",
            NotificationType.AdditionalDocumentsRequired);
    }

    public async Task NotifyClaimDecisionAsync(int claimId, bool approved)
    {
        var claim = await GetClaimWithUserAsync(claimId);
        if (claim == null)
        {
            return;
        }

        await CreateNotificationAsync(
            claim.UserPolicy.Proposal.UserId,
            approved ? "Claim approved" : "Claim rejected",
            approved
                ? $"Your claim {claim.ClaimNumber} has been approved for {claim.ClaimAmount:C}."
                : $"Your claim {claim.ClaimNumber} has been rejected. Please contact support for the detailed denial reason.",
            approved ? NotificationType.ClaimApproved : NotificationType.ClaimRejected);
    }

    public async Task NotifyClaimDisbursementAsync(int claimId, decimal amount, string destination)
    {
        var claim = await GetClaimWithUserAsync(claimId);
        if (claim == null)
        {
            return;
        }

        await CreateNotificationAsync(
            claim.UserPolicy.Proposal.UserId,
            "Claim payout transferred",
            $"A payout of {amount:C} for claim {claim.ClaimNumber} has been transferred to {destination}.",
            NotificationType.ClaimDisbursement);
    }

    public async Task NotifyPasswordResetAsync(string email)
    {
        await NotifyUserByEmailAsync(
            email,
            "Password reset requested",
            "A password reset was requested for your account. If this was not you, contact support immediately.",
            NotificationType.PasswordReset);
    }

    public async Task NotifyTwoFactorAuthenticationAsync(string email)
    {
        await NotifyUserByEmailAsync(
            email,
            "Two-factor authentication code",
            "A two-factor authentication code was requested for your account.",
            NotificationType.TwoFactorAuthentication);
    }

    public async Task NotifySecurityAlertAsync(string email, string? deviceOrLocation)
    {
        var context = string.IsNullOrWhiteSpace(deviceOrLocation)
            ? "an unrecognized device or location"
            : deviceOrLocation;

        await NotifyUserByEmailAsync(
            email,
            "Security alert",
            $"A login attempt was detected from {context}. If this was not you, reset your password immediately.",
            NotificationType.SecurityAlert);
    }

    public async Task<PagedResultDto<NotificationDto>> GetUserNotificationsAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<PagedResultDto<NotificationDto>> GetUserNotificationsByTypeAsync(
        int userId,
        string type,
        PaginationParamsDto paginationParams)
    {
        var query = _context.Notifications
            .Where(x => x.UserId == userId && x.Title.Contains(type))
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(x => x.UserId == userId && !x.IsRead);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Models.Claims.Claim?> GetClaimWithUserAsync(int claimId)
    {
        return await _context.Claims
            .Include(x => x.UserPolicy)
                .ThenInclude(x => x.Proposal)
            .FirstOrDefaultAsync(x => x.ClaimId == claimId);
    }

    private async Task NotifyUserByEmailAsync(string email, string title, string message, NotificationType type)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            return;
        }

        await CreateNotificationAsync(user.UserId, title, message, type);
        await SendEmailAsync(user.Email, title, message);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpSection = _config.GetSection("EmailSettings");
        var host = smtpSection.GetValue<string>("SmtpHost");
        var port = smtpSection.GetValue<int?>("SmtpPort") ?? 587;
        var username = smtpSection.GetValue<string>("Username");
        var password = smtpSection.GetValue<string>("Password");
        var fromEmail = smtpSection.GetValue<string>("FromEmail");
        var fromName = smtpSection.GetValue<string>("FromName") ?? "Vehicle Insurance System";
        var enableSsl = smtpSection.GetValue<bool?>("EnableSsl") ?? true;

        if (IsMissingEmailSetting(host) ||
            IsMissingEmailSetting(username) ||
            IsMissingEmailSetting(password) ||
            IsMissingEmailSetting(fromEmail))
        {
            _logger.LogWarning("Email was not sent because EmailSettings are incomplete.");
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail!, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(username, password)
        };

        try
        {
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}.", toEmail);
            throw;
        }
    }

    private static bool IsMissingEmailSetting(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || value.StartsWith("your-", StringComparison.OrdinalIgnoreCase);
    }
}
