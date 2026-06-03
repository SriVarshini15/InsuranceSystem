namespace VehicleInsuranceSystem.DTOs.AuthDTOs;

public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);