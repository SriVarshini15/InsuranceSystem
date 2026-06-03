using VehicleInsuranceSystem.Models.Users;

namespace VehicleInsuranceSystem.Models.Vehicles;

public class Vehicle
{
    public int VehicleId { get; set; }

    public string VehicleNumber { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int ManufactureYear { get; set; }

    public DateTime PurchaseDate { get; set; }

    public string EngineNumber { get; set; } = string.Empty;

    public string ChassisNumber { get; set; } = string.Empty;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}