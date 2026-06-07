using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.AuthDTOs;
using VehicleInsuranceSystem.DTOs.NotificationDTOs;
using VehicleInsuranceSystem.Interfaces;

namespace VehicleInsuranceSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="registerDto">User registration details</param>
    /// <returns>Registration result with user details</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid registration request model state");
            return BadRequest(new { error = "Invalid registration data", details = ModelState });
        }

        _logger.LogInformation($"Registration attempt for email: {registerDto.Email}");
        return await _authService.RegisterAsync(registerDto);
    }

    /// <summary>
    /// Login user and receive JWT token
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid login request model state");
            return BadRequest(new { error = "Invalid login data", details = ModelState });
        }

        _logger.LogInformation($"Login attempt for email: {loginDto.Email}");
        return await _authService.LoginAsync(loginDto);
    }

    [HttpGet("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        return await _authService.VerifyEmailAsync(token);
    }

    [HttpPost("password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestPasswordReset([FromBody] AuthenticationAlertDto request)
    {
        await _authService.RequestPasswordResetAsync(request.Email);
        return Ok(new { message = "Password reset notification sent if the account exists" });
    }

    [HttpPost("2fa")]
    public async Task<IActionResult> RequestTwoFactorCode([FromBody] AuthenticationAlertDto request)
    {
        await _authService.RequestTwoFactorCodeAsync(request.Email);
        return Ok(new { message = "Two-factor authentication notification sent if the account exists" });
    }

    [HttpPost("security-alert")]
    public async Task<IActionResult> SendSecurityAlert([FromBody] AuthenticationAlertDto request)
    {
        await _authService.SendSecurityAlertAsync(request.Email, request.DeviceOrLocation);
        return Ok(new { message = "Security alert sent if the account exists" });
    }
}
