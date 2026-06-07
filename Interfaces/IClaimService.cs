using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.ClaimDTOs;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.Models.Enums;

namespace VehicleInsuranceSystem.Interfaces;

public interface IClaimService
{
    Task<ClaimDto> CreateClaimAsync(CreateClaimDto createClaimDto);

    Task<PagedResultDto<ClaimDto>> GetClaimsByPolicyIdAsync(int userPolicyId, PaginationParamsDto paginationParams);

    Task<PagedResultDto<ClaimDto>> GetClaimsByUserIdAsync(int userId, PaginationParamsDto paginationParams);

    Task<ClaimDto?> GetClaimByIdAsync(int claimId);

    Task<ClaimDto> UpdateClaimStatusAsync(int claimId, ClaimStatus status);
}
