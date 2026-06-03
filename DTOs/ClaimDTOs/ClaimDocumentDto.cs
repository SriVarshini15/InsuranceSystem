namespace VehicleInsuranceSystem.DTOs.ClaimDTOs;

public record ClaimDocumentDto(
    int ClaimDocumentId,
    string DocumentName,
    string DocumentPath
);