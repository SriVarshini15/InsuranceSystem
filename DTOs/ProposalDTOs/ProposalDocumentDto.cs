namespace VehicleInsuranceSystem.DTOs.ProposalDTOs;

public record ProposalDocumentDto(
    int DocumentId,
    string DocumentName,
    string DocumentPath
);