namespace VehicleInsuranceSystem.DTOs.ClaimDTOs;

public record ClaimDto(
    int ClaimId,
    string ClaimNumber,
    decimal ClaimAmount,
    DateTime IncidentDate,
    string Status
);