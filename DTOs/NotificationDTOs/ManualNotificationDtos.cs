namespace VehicleInsuranceSystem.DTOs.NotificationDTOs;

public record QuoteSavedNotificationDto(
    int ProposalId,
    string? RenewalLink
);

public record AdjusterAssignmentNotificationDto(
    int ClaimId,
    string AdjusterName,
    string ContactInfo
);

public record AdditionalDocumentsNotificationDto(
    int ClaimId,
    string RequiredDocuments
);

public record ClaimDisbursementNotificationDto(
    int ClaimId,
    decimal Amount,
    string Destination
);

public record AuthenticationAlertDto(
    string Email,
    string? DeviceOrLocation
);
