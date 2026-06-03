namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record ProposalDto(
    int ProposalId,
    string ProposalNumber,
    decimal PremiumAmount,
    string Status,
    DateTime SubmittedDate
);