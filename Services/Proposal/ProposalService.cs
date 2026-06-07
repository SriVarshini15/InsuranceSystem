using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.ProposalDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Proposals;

public class ProposalService : IProposalService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ProposalService(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<ProposalDto> CreateProposalAsync(int userId, CreateProposalDto createProposalDto)
    {
        var proposal = _mapper.Map<Proposal>(createProposalDto);
        proposal.UserId = userId;
        proposal.Status = ProposalStatus.ProposalSubmitted;
        proposal.SubmittedDate = DateTime.UtcNow;

        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();
        await _notificationService.NotifyQuoteSavedAsync(proposal.ProposalId);

        return _mapper.Map<ProposalDto>(proposal);
    }

    public async Task<PagedResultDto<ProposalStatusDto>> GetUserProposalsAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.Proposals
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SubmittedDate)
            .ProjectTo<ProposalStatusDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<PagedResultDto<ProposalDto>> GetAllProposalsAsync(PaginationParamsDto paginationParams)
    {
        var query = _context.Proposals
            .OrderByDescending(x => x.SubmittedDate)
            .ProjectTo<ProposalDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<ProposalStatusDto?> GetProposalStatusAsync(int proposalId)
    {
        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        return proposal == null ? null : _mapper.Map<ProposalStatusDto>(proposal);
    }

    public async Task<ProposalDto> ReviewProposalAsync(int proposalId, ReviewProposalDto reviewProposalDto)
    {
        var proposal = await _context.Proposals.FindAsync(proposalId);
        if (proposal == null)
        {
            throw new ResourceNotFoundException("Proposal not found");
        }

        proposal.ReviewedDate = DateTime.UtcNow;
        proposal.Remarks = reviewProposalDto.Remarks;
        proposal.Status = reviewProposalDto.Approved
            ? ProposalStatus.QuoteGenerated
            : ProposalStatus.Rejected;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProposalDto>(proposal);
    }

    public async Task<QuoteDto> GenerateQuoteAsync(CreateQuoteDto createQuoteDto)
    {
        var proposal = await _context.Proposals
            .Include(x => x.Quote)
            .FirstOrDefaultAsync(x => x.ProposalId == createQuoteDto.ProposalId);

        if (proposal == null)
        {
            throw new ResourceNotFoundException("Proposal not found");
        }

        var quote = proposal.Quote ?? new Quote { ProposalId = createQuoteDto.ProposalId };
        quote.BasePremium = createQuoteDto.BasePremium;
        quote.AddonPremium = createQuoteDto.AddonPremium;
        quote.TotalPremium = createQuoteDto.TotalPremium;
        quote.ExpiryDate = createQuoteDto.ExpiryDate;
        quote.Remarks = createQuoteDto.Remarks;
        quote.IsEmailSent = false;

        proposal.Quote = quote;
        proposal.PremiumAmount = createQuoteDto.TotalPremium;
        proposal.Status = ProposalStatus.QuoteGenerated;

        if (quote.QuoteId == 0)
        {
            _context.Quotes.Add(quote);
        }

        await _context.SaveChangesAsync();
        await _notificationService.NotifyQuoteSavedAsync(proposal.ProposalId);

        return _mapper.Map<QuoteDto>(quote);
    }

    public async Task<QuoteDto?> GetQuoteByProposalIdAsync(int proposalId)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        return quote == null ? null : _mapper.Map<QuoteDto>(quote);
    }
}
