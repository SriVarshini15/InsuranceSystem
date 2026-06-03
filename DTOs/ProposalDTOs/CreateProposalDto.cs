namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record CreateProposalDto(
    int VehicleId,
    int PolicyId
);