namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record ProposalStatusDto(
    int ProposalId,
    string ProposalNumber,
    string Status,
    DateTime SubmittedDate,
    DateTime? ReviewedDate,
    string? Remarks
);