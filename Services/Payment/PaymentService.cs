using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PaymentDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Payments;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public PaymentService(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
    {
        var proposal = await _context.Proposals.FindAsync(createPaymentDto.ProposalId);
        if (proposal == null)
        {
            throw new ResourceNotFoundException("Proposal not found");
        }

        var payment = _mapper.Map<Payment>(createPaymentDto);
        payment.PaymentDate = DateTime.UtcNow;
        payment.Status = PaymentStatus.Success;

        _context.Payments.Add(payment);
        proposal.Status = ProposalStatus.PaymentPending;

        await _context.SaveChangesAsync();
        await _notificationService.NotifyPaymentSuccessAsync(payment.ProposalId, payment.Amount);

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto?> GetPaymentByProposalIdAsync(int proposalId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        return payment == null ? null : _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId);

        return payment == null ? null : _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PagedResultDto<PaymentDto>> GetPaymentsByUserIdAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.Payments
            .Include(x => x.Proposal)
            .Where(x => x.Proposal.UserId == userId)
            .OrderByDescending(x => x.PaymentDate)
            .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<PaymentDto> ConfirmPaymentAsync(int paymentId)
    {
        var payment = await _context.Payments
            .Include(x => x.Proposal)
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId);

        if (payment == null)
        {
            throw new ResourceNotFoundException("Payment not found");
        }

        payment.Status = PaymentStatus.Success;
        payment.Proposal.Status = ProposalStatus.Active;

        await _context.SaveChangesAsync();
        await _notificationService.NotifyPaymentSuccessAsync(payment.ProposalId, payment.Amount);
        await _notificationService.NotifyPolicyIssuanceAsync(payment.ProposalId);

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> MarkPaymentFailedAsync(int paymentId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId);

        if (payment == null)
        {
            throw new ResourceNotFoundException("Payment not found");
        }

        payment.Status = PaymentStatus.Failed;
        await _context.SaveChangesAsync();
        await _notificationService.NotifyPaymentFailureAsync(payment.ProposalId, payment.Amount);

        return _mapper.Map<PaymentDto>(payment);
    }
}
