namespace VehicleInsuranceSystem.DTOs.PolicyDTOs;

public record PolicyDto(
    int PolicyId,
    string PolicyName,
    string Description,
    decimal BasePremium,
    decimal CoverageAmount,
    int DurationMonths,
    string Category
);