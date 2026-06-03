namespace VehicleInsuranceSystem.DTOs.VehicleDTOs;

public record VehicleDto(
    int VehicleId,
    string VehicleNumber,
    string VehicleType,
    string Manufacturer,
    string Model,
    int ManufactureYear,
    DateTime PurchaseDate
);