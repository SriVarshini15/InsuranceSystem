namespace VehicleInsuranceSystem.DTOs.UserDTOs;

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address
);