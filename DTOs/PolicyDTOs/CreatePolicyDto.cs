namespace VehicleInsuranceSystem.DTOs.PolicyDTOs;

public record CreatePolicyDto(
    string PolicyName,
    string Description,
    decimal BasePremium,
    decimal CoverageAmount,
    int DurationMonths,
    int PolicyCategoryId
);