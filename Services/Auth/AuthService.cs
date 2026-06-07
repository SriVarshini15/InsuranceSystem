using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.AuthDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Users;

namespace VehicleInsuranceSystem.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly INotificationService _notificationService;

    public AuthService(
        ApplicationDbContext context,
        IMapper mapper,
        IConfiguration config,
        INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _config = config;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> RegisterAsync(RegisterUserDto registerDto)
    {
        var normalizedEmail = NormalizeEmail(registerDto.Email);
        if (await _context.Users.AnyAsync(x => x.Email.ToLower() == normalizedEmail
            || x.AadhaarNumber == registerDto.AadhaarNumber
            || x.PANNumber == registerDto.PANNumber))
        {
            throw new DuplicateResourceException("A user with the same email, Aadhaar, or PAN already exists.");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "User");
        if (role == null)
        {
            role = new Role { RoleName = "User" };
            _context.Roles.Add(role);
        }

        var user = _mapper.Map<User>(registerDto);
        user.Email = normalizedEmail;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        user.Age = CalculateAge(registerDto.DOB);
        user.Role = role;
        user.IsActive = false;
        user.IsEmailVerified = false;
        user.EmailVerificationToken = GenerateSecureToken();
        user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _notificationService.NotifyWelcomeRegistrationAsync(user.UserId);
        await _notificationService.NotifyEmailVerificationAsync(user.Email, BuildEmailVerificationLink(user.EmailVerificationToken));

        var mappedUser = _mapper.Map<UserDto>(user);
        return new OkObjectResult(new
        {
            message = "User registered successfully. Please verify your email before logging in.",
            user = mappedUser
        });
    }

    public async Task<IActionResult> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            if (user != null)
            {
                await _notificationService.NotifySecurityAlertAsync(user.Email, "an unsuccessful login attempt");
            }

            throw new InvalidCredentialsException();
        }

        if (!user.IsEmailVerified)
        {
            throw new ForbiddenException("Please verify your email address before logging in.");
        }

        var token = GenerateJwtToken(user);
        return new OkObjectResult(new AuthResponseDto(
            token,
            user.Role.RoleName,
            user.UserId.ToString(),
            $"{user.FirstName} {user.LastName}"));
    }

    public async Task<IActionResult> VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new BadRequestException("Email verification token is required.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.EmailVerificationToken == token);

        if (user == null)
        {
            throw new BadRequestException("Invalid email verification token.");
        }

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
        {
            throw new BadRequestException("Email verification token has expired. Please register again or request a new verification email.");
        }

        user.IsEmailVerified = true;
        user.IsActive = true;
        user.EmailVerifiedAt = DateTime.UtcNow;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;

        await _context.SaveChangesAsync();

        return new OkObjectResult(new { message = "Email verified successfully. You can now log in." });
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        await _notificationService.NotifyPasswordResetAsync(NormalizeEmail(email));
    }

    public async Task RequestTwoFactorCodeAsync(string email)
    {
        await _notificationService.NotifyTwoFactorAuthenticationAsync(email);
    }

    public async Task SendSecurityAlertAsync(string email, string? deviceOrLocation)
    {
        await _notificationService.NotifySecurityAlertAsync(email, deviceOrLocation);
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age))
        {
            age--;
        }

        return Math.Max(age, 0);
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new BadRequestException("Email is required.");
        }

        try
        {
            var address = new MailAddress(email.Trim());
            if (!string.Equals(address.Address, email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Please enter a valid email address.");
            }

            return address.Address.ToLowerInvariant();
        }
        catch (FormatException)
        {
            throw new BadRequestException("Please enter a valid email address.");
        }
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }

    private string BuildEmailVerificationLink(string? token)
    {
        var baseUrl = _config.GetValue<string>("AppSettings:BaseUrl")?.TrimEnd('/')
            ?? "http://localhost:5092";

        return $"{baseUrl}/api/v1/Auth/verify-email?token={Uri.EscapeDataString(token ?? string.Empty)}";
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSetting = _config.GetSection("JwtSettings");
        var secret = jwtSetting.GetValue<string>("SecretKey");
        var issuer = jwtSetting.GetValue<string>("Issuer");
        var audience = jwtSetting.GetValue<string>("Audience");
        var expiresInMinutes = jwtSetting.GetValue<int>("ExpiresInMinutes");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "DefaultSecretKey123456789012345!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.RoleName),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: issuer ?? "VehicleInsuranceSystem",
            audience: audience ?? "VehicleInsuranceApi",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes > 0 ? expiresInMinutes : 60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
