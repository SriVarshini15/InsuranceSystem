namespace VehicleInsuranceSystem.DTOs.PolicyDTOs;

public record PolicyAddonDto(
    int AddonId,
    string AddonName,
    decimal AddonCost,
    string Description
);