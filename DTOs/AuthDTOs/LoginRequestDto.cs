namespace VehicleInsuranceSystem.DTOs.AuthDTOs;

public record LoginRequestDto(
    string Email,
    string Password
);