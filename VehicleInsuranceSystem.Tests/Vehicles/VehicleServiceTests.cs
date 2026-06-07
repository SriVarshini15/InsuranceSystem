using VehicleInsuranceSystem.DTOs.VehicleDTOs;
using VehicleInsuranceSystem.Services.Vehicles;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Vehicles;

public class VehicleServiceTests : TestBase
{
    [Test]
    public async Task AddVehicleAsync_CreatesVehicle()
    {
        var user = await AddUserAsync();
        var service = new VehicleService(Context, Mapper, Notifications);

        var dto = await service.AddVehicleAsync(user.UserId, CreateVehicle("TN01AB1234"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.VehicleId, Is.GreaterThan(0));
            Assert.That(dto.VehicleNumber, Is.EqualTo("TN01AB1234"));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyPolicyEndorsementAsync)));
        });
    }

    [Test]
    public async Task UpdateVehicleAsync_UpdatesVehicle()
    {
        var user = await AddUserAsync();
        var service = new VehicleService(Context, Mapper, Notifications);
        var vehicle = await service.AddVehicleAsync(user.UserId, CreateVehicle("TN01AB1235"));

        var dto = await service.UpdateVehicleAsync(vehicle.VehicleId, new UpdateVehicleDto("Honda", "City"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.Manufacturer, Is.EqualTo("Honda"));
            Assert.That(dto.Model, Is.EqualTo("City"));
        });
    }

    [Test]
    public async Task DeleteVehicleAsync_RemovesVehicle()
    {
        var user = await AddUserAsync();
        var service = new VehicleService(Context, Mapper, Notifications);
        var vehicle = await service.AddVehicleAsync(user.UserId, CreateVehicle("TN01AB1236"));

        var deleted = await service.DeleteVehicleAsync(vehicle.VehicleId);

        Assert.That(deleted, Is.True);
    }

    private static CreateVehicleDto CreateVehicle(string number)
        => new(number, "Car", "Maruti", "Swift", 2022, new DateTime(2022, 1, 1), $"{number}ENG", $"{number}CHS");
}
