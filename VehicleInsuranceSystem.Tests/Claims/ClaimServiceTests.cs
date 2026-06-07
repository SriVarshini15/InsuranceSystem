using VehicleInsuranceSystem.DTOs.ClaimDTOs;
using VehicleInsuranceSystem.Models.Enums;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Models.UserPolicies;
using VehicleInsuranceSystem.Services.Claims;
using VehicleInsuranceSystem.Tests.TestSupport;

namespace VehicleInsuranceSystem.Tests.Claims;

public class ClaimServiceTests : TestBase
{
    [Test]
    public async Task CreateClaimAsync_SubmitsClaim()
    {
        var userPolicy = await AddUserPolicyAsync();
        var service = new ClaimService(Context, Mapper, Notifications);

        var dto = await service.CreateClaimAsync(new CreateClaimDto(
            userPolicy.UserPolicyId,
            5000,
            DateTime.UtcNow.AddDays(-1),
            "Accident damage"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.ClaimId, Is.GreaterThan(0));
            Assert.That(dto.Status, Is.EqualTo("Initiated"));
            Assert.That(Notifications.Calls, Does.Contain(nameof(Notifications.NotifyClaimIntakeAsync)));
        });
    }

    [TestCase(ClaimStatus.Approved, true)]
    [TestCase(ClaimStatus.Rejected, false)]
    public async Task UpdateClaimStatusAsync_ApprovesOrRejectsClaim(ClaimStatus status, bool approved)
    {
        var userPolicy = await AddUserPolicyAsync();
        var service = new ClaimService(Context, Mapper, Notifications);
        var claim = await service.CreateClaimAsync(new CreateClaimDto(userPolicy.UserPolicyId, 5000, DateTime.UtcNow, "Damage"));

        var dto = await service.UpdateClaimStatusAsync(claim.ClaimId, status);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Status, Is.EqualTo(status.ToString()));
            Assert.That(Notifications.Calls, Does.Contain($"{nameof(Notifications.NotifyClaimDecisionAsync)}:{approved}"));
        });
    }

    private async Task<UserPolicy> AddUserPolicyAsync()
    {
        var user = await AddUserAsync();
        var proposal = new Proposal
        {
            UserId = user.UserId,
            VehicleId = 1,
            PolicyId = 1,
            Status = ProposalStatus.Active,
            SubmittedDate = DateTime.UtcNow
        };
        var userPolicy = new UserPolicy
        {
            Proposal = proposal,
            PolicyNumber = "POL-001",
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddMonths(11),
            Status = "Active"
        };
        Context.UserPolicies.Add(userPolicy);
        await Context.SaveChangesAsync();
        return userPolicy;
    }
}
