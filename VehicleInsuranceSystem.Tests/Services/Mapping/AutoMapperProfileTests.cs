using AutoMapper;
using VehicleInsuranceSystem.Models.Users;
using VehicleInsuranceSystem.Services.Mapping;

namespace VehicleInsuranceSystem.Tests.Services.Mapping;

public class AutoMapperProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = configuration.CreateMapper();
    }

    [TestCase("User")]
    [TestCase("Admin")]
    [TestCase("Officer")]
    public void UserDtoMapping_UsesRoleNameForRole(string roleName)
    {
        var user = new User
        {
            UserId = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@example.com",
            PhoneNumber = "9876543210",
            DOB = new DateTime(1998, 1, 1),
            Age = 28,
            AadhaarNumber = "123412341234",
            PANNumber = "ABCDE1234F",
            Address = "Chennai",
            IsEmailVerified = true,
            RoleId = 2,
            Role = new Role
            {
                RoleId = 2,
                RoleName = roleName
            }
        };

        var dto = _mapper.Map<DTOs.UserDTOs.UserDto>(user);

        Assert.That(dto.Role, Is.EqualTo(roleName));
    }
}
