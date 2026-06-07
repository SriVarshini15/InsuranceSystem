using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.AuthDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IAuthService
{
    Task<IActionResult> RegisterAsync(RegisterUserDto registerDto);

    Task<IActionResult> LoginAsync(LoginRequestDto loginDto);

    Task<IActionResult> VerifyEmailAsync(string token);

    Task RequestPasswordResetAsync(string email);

    Task RequestTwoFactorCodeAsync(string email);

    Task SendSecurityAlertAsync(string email, string? deviceOrLocation);
}
