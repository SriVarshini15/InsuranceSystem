namespace VehicleInsuranceSystem.DTOs.UserPolicyDTOs;

public record UserPolicyDto(
    int UserPolicyId,
    string PolicyNumber,
    DateTime StartDate,
    DateTime EndDate,
    string Status
);