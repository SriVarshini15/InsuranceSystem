using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.Services.Users;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Users;

public class UserServiceTests : TestBase
{
    [Test]
    public async Task GetUserByIdAsync_ReturnsRoleName()
    {
        var user = await AddUserAsync("Officer");
        var service = new UserService(Context, Mapper, Notifications);

        var dto = await service.GetUserByIdAsync(user.UserId);

        Assert.That(dto.Role, Is.EqualTo("Officer"));
    }

    [Test]
    public async Task UpdateUserAsync_UpdatesProfile()
    {
        var user = await AddUserAsync();
        var service = new UserService(Context, Mapper, Notifications);

        var dto = await service.UpdateUserAsync(
            user.UserId,
            new UpdateUserDto("Updated", "Name", "9999999999", "Bengaluru"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo("Updated"));
            Assert.That(dto.Address, Is.EqualTo("Bengaluru"));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyPolicyEndorsementAsync)));
        });
    }

    [Test]
    public async Task DeleteUserAsync_RemovesUser()
    {
        var user = await AddUserAsync();
        var service = new UserService(Context, Mapper, Notifications);

        var deleted = await service.DeleteUserAsync(user.UserId);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.True);
            Assert.That(Context.Users.Find(user.UserId), Is.Null);
        });
    }
}
