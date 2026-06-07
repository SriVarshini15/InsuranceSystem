using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PaymentDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);

    Task<PaymentDto?> GetPaymentByProposalIdAsync(int proposalId);

    Task<PaymentDto?> GetPaymentByIdAsync(int paymentId);

    Task<PagedResultDto<PaymentDto>> GetPaymentsByUserIdAsync(int userId, PaginationParamsDto paginationParams);

    Task<PaymentDto> ConfirmPaymentAsync(int paymentId);

    Task<PaymentDto> MarkPaymentFailedAsync(int paymentId);
}
