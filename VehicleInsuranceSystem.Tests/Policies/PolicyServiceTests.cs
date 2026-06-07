using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.PolicyDTOs;
using VehicleInsuranceSystem.Services.Policies;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Policies;

public class PolicyServiceTests : TestBase
{
    [Test]
    public async Task CreatePolicyAsync_CreatesActivePolicy()
    {
        var category = await AddPolicyCategoryAsync();
        var service = new PolicyService(Context, Mapper);

        var dto = await service.CreatePolicyAsync(CreatePolicy(category.PolicyCategoryId));

        Assert.Multiple(() =>
        {
            Assert.That(dto.PolicyId, Is.GreaterThan(0));
            Assert.That(dto.PolicyName, Is.EqualTo("Secure Drive"));
            Assert.That(dto.Category, Is.EqualTo("Comprehensive"));
            Assert.That(Context.Policies.Single().IsActive, Is.True);
        });
    }

    [Test]
    public async Task GetPolicyByIdAsync_ReturnsPolicy()
    {
        var category = await AddPolicyCategoryAsync();
        var service = new PolicyService(Context, Mapper);
        var created = await service.CreatePolicyAsync(CreatePolicy(category.PolicyCategoryId));

        var dto = await service.GetPolicyByIdAsync(created.PolicyId);

        Assert.That(dto?.PolicyName, Is.EqualTo("Secure Drive"));
    }

    [Test]
    public async Task UpdatePolicyAsync_UpdatesPolicy()
    {
        var category = await AddPolicyCategoryAsync();
        var service = new PolicyService(Context, Mapper);
        var created = await service.CreatePolicyAsync(CreatePolicy(category.PolicyCategoryId));

        var dto = await service.UpdatePolicyAsync(
            created.PolicyId,
            new UpdatePolicyDto("Updated Policy", "Updated", 2000, 200000, 24, false));

        Assert.Multiple(() =>
        {
            Assert.That(dto.PolicyName, Is.EqualTo("Updated Policy"));
            Assert.That(Context.Policies.Single().IsActive, Is.False);
        });
    }

    [Test]
    public async Task GetAllPoliciesAsync_ReturnsPagedPolicies()
    {
        var category = await AddPolicyCategoryAsync();
        var service = new PolicyService(Context, Mapper);
        await service.CreatePolicyAsync(CreatePolicy(category.PolicyCategoryId));

        var page = await service.GetAllPoliciesAsync(new PaginationParamsDto { PageNumber = 1, PageSize = 10 });

        Assert.That(page.TotalCount, Is.EqualTo(1));
    }

    private static CreatePolicyDto CreatePolicy(int categoryId)
        => new("Secure Drive", "Full coverage", 1500, 150000, 12, categoryId);
}
