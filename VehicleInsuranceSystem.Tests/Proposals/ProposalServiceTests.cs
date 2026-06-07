using VehicleInsuranceSystem.DTOs.ProposalDTOs;
using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Models.Vehicles;
using VehicleInsuranceSystem.Services.Proposals;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Proposals;

public class ProposalServiceTests : TestBase
{
    [Test]
    public async Task CreateProposalAsync_SubmitsProposal()
    {
        var (userId, vehicleId, policyId) = await SeedProposalDependenciesAsync();
        var service = new ProposalService(Context, Mapper, Notifications);

        var dto = await service.CreateProposalAsync(userId, new CreateProposalDto(vehicleId, policyId));

        Assert.Multiple(() =>
        {
            Assert.That(dto.ProposalId, Is.GreaterThan(0));
            Assert.That(dto.Status, Is.EqualTo("ProposalSubmitted"));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyQuoteSavedAsync)));
        });
    }

    [Test]
    public async Task GenerateQuoteAsync_SavesPremiumCalculation()
    {
        var (userId, vehicleId, policyId) = await SeedProposalDependenciesAsync();
        var service = new ProposalService(Context, Mapper, Notifications);
        var proposal = await service.CreateProposalAsync(userId, new CreateProposalDto(vehicleId, policyId));

        var quote = await service.GenerateQuoteAsync(new CreateQuoteDto(
            proposal.ProposalId,
            1000,
            250,
            1250,
            DateTime.UtcNow.AddDays(7),
            "Calculated premium"));

        Assert.Multiple(() =>
        {
            Assert.That(quote.TotalPremium, Is.EqualTo(1250));
            Assert.That(Context.Proposals.Find(proposal.ProposalId)!.PremiumAmount, Is.EqualTo(1250));
        });
    }

    private async Task<(int UserId, int VehicleId, int PolicyId)> SeedProposalDependenciesAsync()
    {
        var user = await AddUserAsync();
        var category = await AddPolicyCategoryAsync();
        var vehicle = new Vehicle
        {
            UserId = user.UserId,
            VehicleNumber = $"TN{Guid.NewGuid():N}"[..10],
            VehicleType = "Car",
            Manufacturer = "Maruti",
            Model = "Swift",
            ManufactureYear = 2022,
            PurchaseDate = new DateTime(2022, 1, 1),
            EngineNumber = Guid.NewGuid().ToString("N"),
            ChassisNumber = Guid.NewGuid().ToString("N")
        };
        var policy = new Policy
        {
            PolicyName = "Secure Drive",
            Description = "Full coverage",
            BasePremium = 1000,
            CoverageAmount = 100000,
            DurationMonths = 12,
            IsActive = true,
            PolicyCategoryId = category.PolicyCategoryId,
            PolicyCategory = category
        };

        Context.Vehicles.Add(vehicle);
        Context.Policies.Add(policy);
        await Context.SaveChangesAsync();
        return (user.UserId, vehicle.VehicleId, policy.PolicyId);
    }
}
