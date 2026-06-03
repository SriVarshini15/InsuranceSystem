namespace VehicleInsuranceSystem.DTOs.PolicyDTOs;

public record UpdatePolicyDto(
    string PolicyName,
    string Description,
    decimal BasePremium,
    decimal CoverageAmount,
    int DurationMonths,
    bool IsActive
);