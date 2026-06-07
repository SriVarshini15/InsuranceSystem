using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.DTOs.ProposalDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IProposalService
{
    Task<ProposalDto> CreateProposalAsync(int userId, CreateProposalDto createProposalDto);

    Task<PagedResultDto<ProposalStatusDto>> GetUserProposalsAsync(int userId, PaginationParamsDto paginationParams);

    Task<PagedResultDto<ProposalDto>> GetAllProposalsAsync(PaginationParamsDto paginationParams);

    Task<ProposalStatusDto?> GetProposalStatusAsync(int proposalId);

    Task<ProposalDto> ReviewProposalAsync(int proposalId, ReviewProposalDto reviewProposalDto);

    Task<QuoteDto> GenerateQuoteAsync(CreateQuoteDto createQuoteDto);

    Task<QuoteDto?> GetQuoteByProposalIdAsync(int proposalId);
}
