namespace VehicleInsuranceSystem.DTOs.UserDTOs;

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    DateTime DOB,
    string AadhaarNumber,
    string PANNumber,
    string Address
);