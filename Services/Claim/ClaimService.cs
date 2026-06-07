using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.ClaimDTOs;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Claims;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Claims;

public class ClaimService : IClaimService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ClaimService(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<ClaimDto> CreateClaimAsync(CreateClaimDto createClaimDto)
    {
        var claim = _mapper.Map<Claim>(createClaimDto);
        claim.Status = ClaimStatus.Initiated;

        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
        await _notificationService.NotifyClaimIntakeAsync(claim.ClaimId);

        return _mapper.Map<ClaimDto>(claim);
    }

    public async Task<PagedResultDto<ClaimDto>> GetClaimsByPolicyIdAsync(int userPolicyId, PaginationParamsDto paginationParams)
    {
        var query = _context.Claims
            .Where(x => x.UserPolicyId == userPolicyId)
            .OrderByDescending(x => x.IncidentDate)
            .ProjectTo<ClaimDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<PagedResultDto<ClaimDto>> GetClaimsByUserIdAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.Claims
            .Include(x => x.UserPolicy)
                .ThenInclude(x => x.Proposal)
            .Where(x => x.UserPolicy.Proposal.UserId == userId)
            .OrderByDescending(x => x.IncidentDate)
            .ProjectTo<ClaimDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<ClaimDto?> GetClaimByIdAsync(int claimId)
    {
        var claim = await _context.Claims
            .FirstOrDefaultAsync(x => x.ClaimId == claimId);

        return claim == null ? null : _mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> UpdateClaimStatusAsync(int claimId, ClaimStatus status)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null)
        {
            throw new ResourceNotFoundException("Claim not found");
        }

        claim.Status = status;
        await _context.SaveChangesAsync();

        if (status == ClaimStatus.Approved || status == ClaimStatus.Rejected)
        {
            await _notificationService.NotifyClaimDecisionAsync(claimId, status == ClaimStatus.Approved);
        }

        if (status == ClaimStatus.Paid)
        {
            await _notificationService.NotifyClaimDisbursementAsync(claimId, claim.ClaimAmount, "the registered payout destination");
        }

        return _mapper.Map<ClaimDto>(claim);
    }
}
