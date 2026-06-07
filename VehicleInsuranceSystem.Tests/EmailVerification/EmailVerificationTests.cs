using Microsoft.AspNetCore.Mvc;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Services.Auth;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.EmailVerification;

public class EmailVerificationTests : TestBase
{
    [Test]
    public async Task VerifyEmailAsync_ActivatesUserAndClearsToken()
    {
        var user = await AddUserAsync(verified: false);
        user.EmailVerificationToken = "valid-token";
        user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        await Context.SaveChangesAsync();
        var service = new AuthService(Context, Mapper, Configuration, Notifications);

        var result = await service.VerifyEmailAsync("valid-token");

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.Multiple(() =>
        {
            Assert.That(user.IsEmailVerified, Is.True);
            Assert.That(user.IsActive, Is.True);
            Assert.That(user.EmailVerificationToken, Is.Null);
            Assert.That(user.EmailVerifiedAt, Is.Not.Null);
        });
    }

    [Test]
    public void VerifyEmailAsync_RejectsInvalidToken()
    {
        var service = new AuthService(Context, Mapper, Configuration, Notifications);

        Assert.ThrowsAsync<BadRequestException>(() => service.VerifyEmailAsync("bad-token"));
    }
}
