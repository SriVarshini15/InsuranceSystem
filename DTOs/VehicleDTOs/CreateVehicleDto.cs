namespace VehicleInsuranceSystem.DTOs.VehicleDTOs;

public record CreateVehicleDto(
    string VehicleNumber,
    string VehicleType,
    string Manufacturer,
    string Model,
    int ManufactureYear,
    DateTime PurchaseDate,
    string EngineNumber,
    string ChassisNumber
);