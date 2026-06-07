namespace VehicleInsuranceSystem.DTOs.CommonDTOs;

public record ErrorResponseDto(
    int StatusCode,
    string Title,
    string Message
);
