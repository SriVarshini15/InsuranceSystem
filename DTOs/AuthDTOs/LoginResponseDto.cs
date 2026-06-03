namespace VehicleInsuranceSystem.DTOs.AuthDTOs;

public record LoginResponseDto(
    int UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken
);