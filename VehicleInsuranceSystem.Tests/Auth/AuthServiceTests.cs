using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.DTOs.AuthDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.Services.Auth;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Auth;

public class AuthServiceTests : TestBase
{
    [Test]
    public async Task RegisterAsync_CreatesInactiveUserAndSendsVerification()
    {
        var service = new AuthService(Context, Mapper, Configuration, Notifications);
        var request = new RegisterUserDto(
            "Ravi",
            "Kumar",
            "RAVI@example.com",
            "Password@123",
            "9876543210",
            new DateTime(1998, 1, 1),
            "123412341234",
            "ABCDE1234F",
            "Chennai");

        var result = await service.RegisterAsync(request);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var user = Context.Users.Single();
        Assert.Multiple(() =>
        {
            Assert.That(user.Email, Is.EqualTo("ravi@example.com"));
            Assert.That(user.IsEmailVerified, Is.False);
            Assert.That(user.EmailVerificationToken, Is.Not.Null.And.Not.Empty);
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyWelcomeRegistrationAsync)));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyEmailVerificationAsync)));
        });
    }

    [Test]
    public async Task LoginAsync_ReturnsJwtForVerifiedUser()
    {
        var user = await AddUserAsync("Admin");
        var service = new AuthService(Context, Mapper, Configuration, Notifications);

        var result = await service.LoginAsync(new LoginRequestDto(user.Email, "Password@123"));

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var response = ok!.Value as AuthResponseDto;
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Token, Is.Not.Empty);
            Assert.That(response.Role, Is.EqualTo("Admin"));
            Assert.That(response.UserId, Is.EqualTo(user.UserId.ToString()));
        });
    }
}
