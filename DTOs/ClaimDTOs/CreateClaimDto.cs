namespace VehicleInsuranceSystem.DTOs.ClaimDTOs;

public record CreateClaimDto(
    int UserPolicyId,
    decimal ClaimAmount,
    DateTime IncidentDate,
    string Description
);