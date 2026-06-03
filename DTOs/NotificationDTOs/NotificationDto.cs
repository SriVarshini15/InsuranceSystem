namespace VehicleInsuranceSystem.DTOs.NotificationDTOs;

public record NotificationDto(
    int NotificationId,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);