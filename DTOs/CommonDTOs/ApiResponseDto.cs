namespace VehicleInsuranceSystem.DTOs.CommonDTOs;

public record ApiResponseDto<T>(
    bool Success,
    string Message,
    T? Data
);