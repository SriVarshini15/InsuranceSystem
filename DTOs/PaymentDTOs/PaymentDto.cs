namespace VehicleInsuranceSystem.DTOs.PaymentDTOs;

public record PaymentDto(
    int PaymentId,
    decimal Amount,
    string TransactionNumber,
    DateTime PaymentDate,
    string Status
);