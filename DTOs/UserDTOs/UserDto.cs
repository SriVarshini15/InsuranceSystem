namespace VehicleInsuranceSystem.DTOs.UserDTOs;

public record UserDto(
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DOB,
    int Age,
    string AadhaarNumber,
    string PANNumber,
    string Address,
    string Role
);