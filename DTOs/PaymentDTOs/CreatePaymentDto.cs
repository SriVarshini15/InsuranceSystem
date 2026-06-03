namespace VehicleInsuranceSystem.DTOs.PaymentDTOs;

public record CreatePaymentDto(
    int ProposalId,
    decimal Amount,
    string PaymentMethod
);