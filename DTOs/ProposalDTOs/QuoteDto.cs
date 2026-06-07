namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record QuoteDto(
    int QuoteId,
    string QuoteNumber,
    int ProposalId,
    decimal BasePremium,
    decimal AddonPremium,
    decimal TotalPremium,
    DateTime GeneratedDate,
    DateTime? ExpiryDate,
    bool IsEmailSent,
    string Remarks
);
