using AutoMapper;
using VehicleInsuranceSystem.DTOs.AuthDTOs;
using VehicleInsuranceSystem.DTOs.ClaimDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.DTOs.PaymentDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.DTOs.ProposalDTOs;
using VehicleInsuranceSystem.DTOs.UserPolicyDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.DTOs.VehicleDTOs;
using VehicleInsuranceSystem.Models.Claims;
using VehicleInsuranceSystem.Models.Notifications;
using VehicleInsuranceSystem.Models.Payments;
using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Models.UserPolicies;
using VehicleInsuranceSystem.Models.Users;
using VehicleInsuranceSystem.Models.Vehicles;

namespace VehicleInsuranceSystem.Services.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Age, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore())
            .ForMember(dest => dest.Proposals, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

        CreateMap<User, UserDto>()
            .ForCtorParam(nameof(UserDto.Role), opt => opt.MapFrom(src => src.Role.RoleName));

        CreateMap<CreateVehicleDto, Vehicle>();
        CreateMap<Vehicle, VehicleDto>();

        CreateMap<CreatePolicyDto, Policy>();
        CreateMap<Policy, PolicyDto>()
            .ForCtorParam(nameof(PolicyDto.Category), opt => opt.MapFrom(src => src.PolicyCategory.CategoryName));

        CreateMap<CreateProposalDto, Proposal>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SubmittedDate, opt => opt.Ignore())
            .ForMember(dest => dest.PremiumAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Quote, opt => opt.Ignore());

        CreateMap<Proposal, ProposalDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Proposal, ProposalStatusDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateQuoteDto, Quote>();
        CreateMap<Quote, QuoteDto>();

        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.PaymentDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateClaimDto, Claim>()
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<Claim, ClaimDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Notification, NotificationDto>();

        CreateMap<UserPolicy, UserPolicyDto>();
    }
}
