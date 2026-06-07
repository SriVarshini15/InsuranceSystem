using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Models.Users;
using VehicleInsuranceSystem.Services.Mapping;

namespace VehicleInsuranceSystem.Tests.TestSupport;

public abstract class TestBase
{
    protected ApplicationDbContext Context { get; private set; } = null!;
    protected IMapper Mapper { get; private set; } = null!;
    protected FakeNotificationService Notifications { get; private set; } = null!;
    protected IConfiguration Configuration { get; private set; } = null!;

    [SetUp]
    public virtual void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();
        Notifications = new FakeNotificationService();
        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "TestSecretKey12345678901234567890!",
                ["JwtSettings:Issuer"] = "VehicleInsuranceSystem.Tests",
                ["JwtSettings:Audience"] = "VehicleInsuranceApi.Tests",
                ["JwtSettings:ExpiresInMinutes"] = "60",
                ["AppSettings:BaseUrl"] = "http://localhost:5092"
            })
            .Build();
    }

    [TearDown]
    public virtual void TearDown()
    {
        Context.Dispose();
    }

    protected async Task<Role> AddRoleAsync(string roleName = "User")
    {
        var role = new Role { RoleName = roleName };
        Context.Roles.Add(role);
        await Context.SaveChangesAsync();
        return role;
    }

    protected async Task<User> AddUserAsync(string roleName = "User", bool verified = true)
    {
        var role = await AddRoleAsync(roleName);
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"test{Guid.NewGuid():N}@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            PhoneNumber = "9876543210",
            DOB = new DateTime(1998, 1, 1),
            Age = 28,
            AadhaarNumber = Guid.NewGuid().ToString("N")[..12],
            PANNumber = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant(),
            Address = "Chennai",
            IsActive = verified,
            IsEmailVerified = verified,
            Role = role
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    protected async Task<PolicyCategory> AddPolicyCategoryAsync(string name = "Comprehensive")
    {
        var category = new PolicyCategory { CategoryName = name };
        Context.PolicyCategories.Add(category);
        await Context.SaveChangesAsync();
        return category;
    }
}
