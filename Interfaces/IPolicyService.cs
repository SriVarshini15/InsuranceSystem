using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.DTOs.UserPolicyDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IPolicyService
{
    Task<PagedResultDto<PolicyDto>> GetAllPoliciesAsync(PaginationParamsDto paginationParams);

    Task<PagedResultDto<PolicyDto>> GetActivePoliciesAsync(PaginationParamsDto paginationParams);

    Task<PolicyDto?> GetPolicyByIdAsync(int policyId);

    Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto createPolicyDto);

    Task<PolicyDto> UpdatePolicyAsync(int policyId, UpdatePolicyDto updatePolicyDto);

    Task<PolicyDto> SetPolicyActiveStatusAsync(int policyId, bool isActive);

    Task<PagedResultDto<UserPolicyDto>> GetUserPoliciesAsync(int userId, PaginationParamsDto paginationParams);

    Task<UserPolicyDto?> GetUserPolicyByIdAsync(int userPolicyId);

    Task<byte[]?> GeneratePolicyDocumentAsync(int userPolicyId);
}
