using Microsoft.Extensions.Logging.Abstractions;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Services.Notifications;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Notifications;

public class NotificationServiceTests : TestBase
{
    [Test]
    public async Task CreateNotificationAsync_CreatesUnreadNotification()
    {
        var user = await AddUserAsync();
        var service = new NotificationService(Context, Mapper, Configuration, NullLogger<NotificationService>.Instance);

        var dto = await service.CreateNotificationAsync(user.UserId, "Title", "Message", NotificationType.PaymentReceived);

        Assert.Multiple(() =>
        {
            Assert.That(dto.NotificationId, Is.GreaterThan(0));
            Assert.That(dto.Title, Is.EqualTo("Title"));
            Assert.That(dto.IsRead, Is.False);
            Assert.That(Context.Notifications.Count(), Is.EqualTo(1));
        });
    }
}
