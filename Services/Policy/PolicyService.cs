using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.DTOs.UserPolicyDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Policies;

public class PolicyService : IPolicyService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PolicyService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<PolicyDto>> GetAllPoliciesAsync(PaginationParamsDto paginationParams)
    {
        var query = _context.Policies
            .Include(x => x.PolicyCategory)
            .OrderBy(x => x.PolicyId)
            .ProjectTo<PolicyDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<PagedResultDto<PolicyDto>> GetActivePoliciesAsync(PaginationParamsDto paginationParams)
    {
        var query = _context.Policies
            .Where(x => x.IsActive)
            .Include(x => x.PolicyCategory)
            .OrderBy(x => x.PolicyId)
            .ProjectTo<PolicyDto>(_mapper.ConfigurationProvider)
            ;

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }


    public async Task<PolicyDto?> GetPolicyByIdAsync(int policyId)
    {
        var policy = await _context.Policies
            .Include(x => x.PolicyCategory)
            .FirstOrDefaultAsync(x => x.PolicyId == policyId);

        return policy == null ? null : _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
    {
        var policy = _mapper.Map<Policy>(createPolicyDto);
        policy.IsActive = true;
        policy.PolicyCategory = await _context.PolicyCategories
            .FirstAsync(x => x.PolicyCategoryId == createPolicyDto.PolicyCategoryId);

        _context.Policies.Add(policy);
        await _context.SaveChangesAsync();

        return _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PolicyDto> UpdatePolicyAsync(int policyId, UpdatePolicyDto updatePolicyDto)
    {
        var policy = await _context.Policies
            .Include(x => x.PolicyCategory)
            .FirstOrDefaultAsync(x => x.PolicyId == policyId);
        if (policy == null)
        {
            throw new ResourceNotFoundException("Policy not found");
        }

        policy.PolicyName = updatePolicyDto.PolicyName;
        policy.Description = updatePolicyDto.Description;
        policy.BasePremium = updatePolicyDto.BasePremium;
        policy.CoverageAmount = updatePolicyDto.CoverageAmount;
        policy.DurationMonths = updatePolicyDto.DurationMonths;
        policy.IsActive = updatePolicyDto.IsActive;

        await _context.SaveChangesAsync();

        return _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PolicyDto> SetPolicyActiveStatusAsync(int policyId, bool isActive)
    {
        var policy = await _context.Policies
            .Include(x => x.PolicyCategory)
            .FirstOrDefaultAsync(x => x.PolicyId == policyId);

        if (policy == null)
        {
            throw new ResourceNotFoundException("Policy not found");
        }

        policy.IsActive = isActive;
        await _context.SaveChangesAsync();

        return _mapper.Map<PolicyDto>(policy);
    }

    public async Task<PagedResultDto<UserPolicyDto>> GetUserPoliciesAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.UserPolicies
            .Include(x => x.Proposal)
            .Where(x => x.Proposal.UserId == userId)
            .OrderByDescending(x => x.StartDate)
            .ProjectTo<UserPolicyDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<UserPolicyDto?> GetUserPolicyByIdAsync(int userPolicyId)
    {
        var userPolicy = await _context.UserPolicies
            .FirstOrDefaultAsync(x => x.UserPolicyId == userPolicyId);

        return userPolicy == null ? null : _mapper.Map<UserPolicyDto>(userPolicy);
    }

    public async Task<byte[]?> GeneratePolicyDocumentAsync(int userPolicyId)
    {
        var userPolicy = await _context.UserPolicies
            .Include(x => x.Proposal)
                .ThenInclude(x => x.User)
            .Include(x => x.Proposal)
                .ThenInclude(x => x.Policy)
            .FirstOrDefaultAsync(x => x.UserPolicyId == userPolicyId);

        if (userPolicy == null)
        {
            return null;
        }

        var document = $"""
        Vehicle Insurance Policy Document

        Policy Number: {userPolicy.PolicyNumber}
        Customer: {userPolicy.Proposal.User.FirstName} {userPolicy.Proposal.User.LastName}
        Policy: {userPolicy.Proposal.Policy.PolicyName}
        Status: {userPolicy.Status}
        Start Date: {userPolicy.StartDate:yyyy-MM-dd}
        End Date: {userPolicy.EndDate:yyyy-MM-dd}
        Premium Paid: {userPolicy.Proposal.PremiumAmount:C}
        """;

        return System.Text.Encoding.UTF8.GetBytes(document);
    }
}
