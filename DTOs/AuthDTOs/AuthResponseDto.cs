namespace VehicleInsuranceSystem.DTOs.AuthDTOs;

public record AuthResponseDto(
    string Token,
    string Role,
    string UserId,
    string UserName
);
