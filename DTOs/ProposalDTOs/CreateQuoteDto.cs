namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record CreateQuoteDto(
    int ProposalId,
    decimal BasePremium,
    decimal AddonPremium,
    decimal TotalPremium,
    DateTime? ExpiryDate,
    string Remarks
);
