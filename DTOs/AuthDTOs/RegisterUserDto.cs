namespace VehicleInsuranceSystem.DTOs.AuthDTOs;

public record RegisterUserDto(
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